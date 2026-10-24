using UC;
using UnityEngine;

// The actor. Animation goes through a PlayableHandler on the model (no Animator controller): idle while
// standing, walk while moving, cross-fading between them.
[RequireComponent(typeof(MovementDirectionalXZ))]
public class Player : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField, Tooltip("On the model; the first one in the children if left empty")]
    private PlayableHandler playableHandler;
    [SerializeField]
    private AnimationClip   idleAnim;
    [SerializeField]
    private AnimationClip   walkAnim;
    [SerializeField, Min(0), Tooltip("Cross-fade time between animations, in seconds")]
    private float           transitionTime = 0.15f;

    MovementDirectionalXZ   movement;

    void Start()
    {
        movement = GetComponent<MovementDirectionalXZ>();
        if (playableHandler == null) playableHandler = GetComponentInChildren<PlayableHandler>();
    }

    void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (playableHandler == null) return;

        AnimationClip clip = (movement.isMoving) ? (walkAnim) : (idleAnim);
        if (clip == null) return;

        // While cross-fading, the clip that counts is the one fading in
        AnimationClip playing = (playableHandler.IsTransitioning) ? (playableHandler.TransitionToClip) : (playableHandler.currentClip);
        if (playing == clip) return;

        playableHandler.Play(clip, false, transitionTime);
    }
}
