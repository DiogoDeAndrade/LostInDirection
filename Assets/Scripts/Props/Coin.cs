using UC;
using UnityEngine;

// A coin to collect: it spins and bobs where it is, and when the actor walks into it, it pops (swells and shrinks
// away) and is gone, with its sound, a little higher for each coin picked up soon after the last (a streak). Nothing
// else; it's there so exploring feels rewarded.
public class Coin : MonoBehaviour
{
    [SerializeField, Tooltip("Degrees per second")]
    private float   spinSpeed = 120.0f;
    [SerializeField]
    private float   bobHeight = 0.05f;
    [SerializeField]
    private float   bobSpeed = 2.0f;
    [SerializeField, Min(0.0f), Tooltip("How close (on the ground plane) the actor has to come")]
    private float   pickupRadius = 0.35f;
    [SerializeField, Min(0.01f)]
    private float   popTime = 0.25f;
    [SerializeField]
    private SoundDef    pickupSound;
    [SerializeField, Min(0.0f), Tooltip("Seconds within which the next coin carries on the streak")]
    private float   streakTime = 1.5f;
    [SerializeField, Min(0.0f), Tooltip("Pitch added per coin of a streak")]
    private float   streakPitchStep = 0.06f;
    [SerializeField, Min(1.0f)]
    private float   streakPitchMax = 1.5f;

    static float    lastPickupTime = float.MinValue;
    static int      streak;

    // Statics survive Play mode without a domain reload
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStreak()
    {
        lastPickupTime = float.MinValue;
        streak = 0;
    }

    Transform   actor;
    Vector3     basePosition;
    Vector3     baseScale;
    float       bobOffset;
    float       popElapsed = -1.0f;

    void Start()
    {
        var player = FindAnyObjectByType<Player>();
        if (player != null) actor = player.transform;
        basePosition = transform.position;
        baseScale = transform.localScale;
        bobOffset = Random.Range(0.0f, Mathf.PI * 2.0f);
        transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f, Space.World);
    }

    void Update()
    {
        transform.Rotate(0.0f, spinSpeed * Time.deltaTime, 0.0f, Space.World);

        if (popElapsed >= 0.0f)
        {
            popElapsed += Time.deltaTime;
            float k = popElapsed / popTime;
            transform.localScale = baseScale * ((k < 0.4f) ? (Mathf.Lerp(1.0f, 1.6f, k / 0.4f)) : (Mathf.Lerp(1.6f, 0.0f, (k - 0.4f) / 0.6f)));
            transform.position = basePosition + Vector3.up * (k * 0.4f);
            if (k >= 1.0f) Destroy(gameObject);
            return;
        }

        transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobHeight);

        if (actor == null) return;
        Vector3 toActor = actor.position - basePosition;
        toActor.y = 0.0f;
        if (toActor.sqrMagnitude < pickupRadius * pickupRadius) PickUp();
    }

    void PickUp()
    {
        popElapsed = 0.0f;
        streak = (Time.time - lastPickupTime <= streakTime) ? (streak + 1) : (0);
        lastPickupTime = Time.time;
        if (pickupSound != null) pickupSound.Play(1.0f, Mathf.Min(1.0f + streak * streakPitchStep, streakPitchMax));
    }
}
