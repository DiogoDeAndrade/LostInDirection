using UC;
using UnityEngine;
using UnityEngine.AI;

// Idle/walk animation through a PlayableHandler (no Animator controller), picked from how fast the character moves:
// a NavMeshAgent's velocity, a non-kinematic Rigidbody's, or otherwise how far it moved. One-shot clips (played by
// actions) take over until they finish, then idle/walk resumes.
public class CharacterAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("On the model; the first one in the children if left empty")]
    private PlayableHandler playableHandler;
    [SerializeField]
    private AnimationClip   idleAnim;
    [SerializeField]
    private AnimationClip   walkAnim;
    [SerializeField, Min(0), Tooltip("Cross-fade time between idle and walk, in seconds")]
    private float           transitionTime = 0.15f;
    [SerializeField, Min(0), Tooltip("Ground speed (units/s) above which the character walks")]
    private float           walkThreshold = 0.1f;

    NavMeshAgent    agent;
    Rigidbody       rb;
    Vector3         lastPosition;
    AnimationClip   oneShot;

    public bool isPlayingOneShot => oneShot != null;

    void Start()
    {
        if (playableHandler == null) playableHandler = GetComponentInChildren<PlayableHandler>();
        if (playableHandler != null) playableHandler.onAnimationComplete += OnAnimationComplete;

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void OnDestroy()
    {
        if (playableHandler != null) playableHandler.onAnimationComplete -= OnAnimationComplete;
    }

    void Update()
    {
        float speed = GetGroundSpeed();
        if ((playableHandler == null) || (oneShot != null)) return;

        AnimationClip clip = (speed > walkThreshold) ? (walkAnim) : (idleAnim);
        if (clip == null) return;

        // While cross-fading, the clip that counts is the one fading in
        AnimationClip playing = (playableHandler.IsTransitioning) ? (playableHandler.TransitionToClip) : (playableHandler.currentClip);
        if (playing == clip) return;

        playableHandler.Play(clip, false, transitionTime);
    }

    float GetGroundSpeed()
    {
        Vector3 velocity;
        if ((agent != null) && (agent.enabled)) velocity = agent.velocity;
        else if ((rb != null) && (!rb.isKinematic)) velocity = rb.linearVelocity;
        else velocity = (Time.deltaTime > 0.0f) ? ((transform.position - lastPosition) / Time.deltaTime) : (Vector3.zero);

        lastPosition = transform.position;
        velocity.y = 0.0f;
        return velocity.magnitude;
    }

    // Plays a clip once, over idle/walk; returns how long it lasts
    public float PlayOneShot(AnimationClip clip, float transition)
    {
        if ((clip == null) || (playableHandler == null)) return 0.0f;

        oneShot = clip;
        playableHandler.Play(clip, false, transition);
        return clip.length;
    }

    void OnAnimationComplete(AnimationClip clip)
    {
        if (clip == oneShot) oneShot = null;
    }
}
