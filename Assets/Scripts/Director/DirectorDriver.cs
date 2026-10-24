using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

// Drives the director (in the Director scene) from what the actor does about the current target, which the movie's
// script sets with the Set Player Target action as it goes on. Frustration never jumps: it only moves at set rates.
// - Far from the target (beyond nearDistance) it follows progress: down while the actor gets closer, up while he gets
//   farther, and slowly up while he makes no progress. The distance can be measured along the NavMesh, so going
//   round an obstacle doesn't count as moving away.
// - Near the target it follows the aim: down while the actor aims at the target (it's what he'd interact with, or,
//   aiming at nothing, he faces it), up while he faces away, and faster while he's aiming at another object.
// - Frustration above moodThreshold for longer than moodGraceTime makes the mood drop.
// Without a target the director calms down; during cutscenes nothing changes. One per movie scene.
public class DirectorDriver : MonoBehaviour
{
    public enum State { NoTarget = 0, Far = 1, Near = 2, Paused = 3 };

    [Header("Start")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float   startFrustration = 0.3f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float   startMood = 1.0f;

    [Header("Far: getting there")]
    [SerializeField, Min(0), Tooltip("Beyond this distance from the target it's about getting there; closer, about aiming at it")]
    private float   nearDistance = 1.2f;
    [SerializeField, Tooltip("Measure the distance along the NavMesh instead of in a straight line")]
    private bool    usePathDistance = true;
    [SerializeField, Min(0.02f), Tooltip("Seconds between distance measurements")]
    private float   measureInterval = 0.2f;
    [SerializeField, Min(0), Tooltip("Units per second towards or away from the target that count as moving; slower is standing still")]
    private float   progressThreshold = 0.2f;
    [SerializeField, Min(0), Tooltip("Frustration lost per second while getting closer")]
    private float   approachRate = 0.3f;
    [SerializeField, Min(0), Tooltip("Frustration gained per second while getting farther")]
    private float   retreatRate = 0.4f;
    [SerializeField, Min(0), Tooltip("Frustration gained per second while far and not getting closer")]
    private float   idleRate = 0.08f;

    [Header("Near: aiming")]
    [SerializeField, Range(0.0f, 180.0f), Tooltip("When the actor isn't aiming at any object: facing within this angle of the target counts as aiming at it (the actor's own interaction angle is 45)")]
    private float   aimAngle = 45.0f;
    [SerializeField, Min(0), Tooltip("Frustration lost per second while aiming at the target")]
    private float   aimRate = 0.4f;
    [SerializeField, Min(0), Tooltip("Frustration gained per second while facing away from it")]
    private float   lookAwayRate = 0.25f;
    [SerializeField, Min(0), Tooltip("Frustration gained per second while aiming at another object")]
    private float   wrongObjectRate = 0.5f;

    [Header("Mood")]
    [SerializeField, Range(0.0f, 1.0f), Tooltip("Frustration above this makes the mood drop, after the grace time")]
    private float   moodThreshold = 0.7f;
    [SerializeField, Min(0), Tooltip("Seconds frustration can stay above the threshold before the mood starts dropping")]
    private float   moodGraceTime = 2.0f;
    [SerializeField, Min(0), Tooltip("Mood lost per second after that")]
    private float   moodDecayRate = 0.05f;

    [Header("No target")]
    [SerializeField, Min(0), Tooltip("Frustration lost per second while there's no target")]
    private float   relaxRate = 0.2f;

    public static DirectorDriver instance { get; private set; }

    Player          actor;
    Interactable    targetInteractable;
    Transform       targetTransform;
    NavMeshPath     path;
    float           lastDistance = -1.0f;
    float           measureElapsed;
    float           progressSpeed;
    float           timeAboveThreshold;

    [ShowNativeProperty]
    public State state { get; private set; }
    [ShowNativeProperty]
    public float frustration { get; private set; }
    [ShowNativeProperty]
    public float mood { get; private set; }
    // Last measured distance to the target (along the NavMesh when that's on and works)
    [ShowNativeProperty]
    public float distance { get; private set; }
    // How fast the actor is getting closer (negative: farther), in units per second
    [ShowNativeProperty]
    public float progress => progressSpeed;

    public bool hasTarget => (targetInteractable != null) || (targetTransform != null);

    void Awake()
    {
        instance = this;
        path = new NavMeshPath();
        frustration = startFrustration;
        mood = startMood;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Start()
    {
        actor = FindAnyObjectByType<Player>();
    }

    // The interactable to go to and aim at, or (when that's null) a place; both null clears the target
    public void SetTarget(Interactable interactable, Transform place)
    {
        targetInteractable = interactable;
        targetTransform = (interactable != null) ? (interactable.transform) : (place);
        lastDistance = -1.0f;
        measureElapsed = 0.0f;
        progressSpeed = 0.0f;
    }

    // For right and wrong actions
    public void ChangeMood(float delta)
    {
        mood = Mathf.Clamp01(mood + delta);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (Cutscene.isActive)
        {
            state = State.Paused;
        }
        else if ((!hasTarget) || (actor == null))
        {
            state = State.NoTarget;
            ChangeFrustration(-relaxRate * dt);
        }
        else
        {
            Vector3 actorPos = actor.transform.position;
            Vector3 point = GetTargetPoint(actorPos);
            Vector3 toTarget = point - actorPos;
            toTarget.y = 0.0f;

            if (toTarget.magnitude > nearDistance)
            {
                state = State.Far;
                UpdateFar(actorPos, point, dt);
            }
            else
            {
                state = State.Near;
                lastDistance = -1.0f;
                progressSpeed = 0.0f;
                UpdateNear(toTarget, dt);
            }
            UpdateMood(dt);
        }

        var director = Director.instance;
        if (director != null)
        {
            director.frustration = frustration;
            director.mood = mood;
        }
    }

    void UpdateFar(Vector3 actorPos, Vector3 point, float dt)
    {
        measureElapsed += dt;
        if ((lastDistance < 0.0f) || (measureElapsed >= measureInterval))
        {
            float d = MeasureDistance(actorPos, point, out bool comparable);
            progressSpeed = ((lastDistance >= 0.0f) && (comparable)) ? ((lastDistance - d) / measureElapsed) : (0.0f);
            lastDistance = d;
            distance = d;
            measureElapsed = 0.0f;
        }

        if (progressSpeed > progressThreshold) ChangeFrustration(-approachRate * dt);
        else if (progressSpeed < -progressThreshold) ChangeFrustration(retreatRate * dt);
        else ChangeFrustration(idleRate * dt);
    }

    void UpdateNear(Vector3 toTarget, float dt)
    {
        distance = toTarget.magnitude;

        // When the actor is aiming at an object (the one he'd interact with), that decides: the target, or another one.
        // The angle below only counts when he's aiming at nothing, since its measure (to the closest point of the
        // target, within aimAngle) doesn't always agree with the actor's own targeting.
        var aimed = actor.currentTarget;
        if (aimed != null)
        {
            ChangeFrustration((aimed == targetInteractable) ? (-aimRate * dt) : (wrongObjectRate * dt));
            return;
        }

        Vector3 forward = actor.transform.forward;
        forward.y = 0.0f;
        bool facing = (toTarget.sqrMagnitude < 0.0001f) || (Vector3.Angle(forward, toTarget) <= aimAngle);
        ChangeFrustration((facing) ? (-aimRate * dt) : (lookAwayRate * dt));
    }

    void UpdateMood(float dt)
    {
        if (frustration > moodThreshold)
        {
            timeAboveThreshold += dt;
            if (timeAboveThreshold > moodGraceTime) mood = Mathf.Clamp01(mood - moodDecayRate * dt);
        }
        else
        {
            timeAboveThreshold = 0.0f;
        }
    }

    void ChangeFrustration(float delta)
    {
        frustration = Mathf.Clamp01(frustration + delta);
    }

    Vector3 GetTargetPoint(Vector3 actorPos)
    {
        return (targetInteractable != null) ? (targetInteractable.ClosestPoint(actorPos)) : (targetTransform.position);
    }

    // Distance from the actor to the target; comparable is false when this measurement can't be compared with the
    // last one (the path failed, so it fell back to a straight line)
    float MeasureDistance(Vector3 from, Vector3 to, out bool comparable)
    {
        Vector3 straight = to - from;
        straight.y = 0.0f;
        comparable = true;
        if (!usePathDistance) return straight.magnitude;

        if ((NavMesh.SamplePosition(from, out var fromHit, 1.0f, NavMesh.AllAreas)) &&
            (NavMesh.SamplePosition(to, out var toHit, 2.0f, NavMesh.AllAreas)) &&
            (NavMesh.CalculatePath(fromHit.position, toHit.position, NavMesh.AllAreas, path)) &&
            (path.status == NavMeshPathStatus.PathComplete))
        {
            float length = 0.0f;
            var corners = path.corners;
            for (int i = 1; i < corners.Length; i++) length += Vector3.Distance(corners[i - 1], corners[i]);
            // The target point itself may be off the NavMesh (inside the object)
            return length + Vector3.Distance(toHit.position, to);
        }

        comparable = false;
        return straight.magnitude;
    }
}
