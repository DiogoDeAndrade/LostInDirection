using NaughtyAttributes;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;

// The actor. Animation goes through a PlayableHandler on the model (no Animator controller): idle while
// standing, walk while moving, cross-fading between them.
// Interaction: the closest-to-facing Interactable in range is the target (outlined, with the prompt next to it);
// one button cycles its verbs, another performs the selected one.
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

    [Header("Interaction")]
    [SerializeField, Min(0), Tooltip("How far the actor can be from an object (its collider) to interact with it")]
    private float               interactRange = 0.6f;
    [SerializeField, Range(0.0f, 180.0f), Tooltip("How far off the actor's facing an object can be, in degrees")]
    private float               interactAngle = 45.0f;
    [SerializeField, ShowIf(nameof(needNewInputSystem))]
    private PlayerInput         playerInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton, Tooltip("Performs the selected verb")]
    private UC.InputControl     interactInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton, Tooltip("Selects the next verb")]
    private UC.InputControl     nextVerbInput;
    [SerializeField, Tooltip("The first one in the scene if left empty")]
    private InteractionPrompt   prompt;

    MovementDirectionalXZ   movement;
    Interactable            target;
    int                     verbIndex;

    public bool needNewInputSystem => (interactInput != null) && (interactInput.type == UC.InputControl.InputType.NewInput);

    // What the actor is facing and can interact with (null when nothing is)
    public Interactable currentTarget => target;

    void Start()
    {
        movement = GetComponent<MovementDirectionalXZ>();
        if (playableHandler == null) playableHandler = GetComponentInChildren<PlayableHandler>();
        if (prompt == null) prompt = FindAnyObjectByType<InteractionPrompt>();

        if (interactInput != null) interactInput.playerInput = playerInput;
        if (nextVerbInput != null) nextVerbInput.playerInput = playerInput;
    }

    void Update()
    {
        UpdateAnimation();
        UpdateInteraction();
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

    void UpdateInteraction()
    {
        Interactable newTarget = FindTarget();
        if (newTarget != target)
        {
            if (target) target.SetHighlight(false);
            target = newTarget;
            verbIndex = 0;
            if (target) target.SetHighlight(true);
        }

        if (target != null)
        {
            if (nextVerbInput.IsDown()) verbIndex = (verbIndex + 1) % target.verbs.Count;
            if (interactInput.IsDown()) target.Interact(this, verbIndex);
        }

        if (prompt != null)
        {
            bool gamepad = (playerInput != null) && (playerInput.currentControlScheme == "Gamepad");
            prompt.Show(target, verbIndex, gamepad);
        }
    }

    // The interactable in range that is closest to straight ahead
    Interactable FindTarget()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;
        forward.y = 0.0f;

        Interactable best = null;
        float bestAngle = float.MaxValue;

        foreach (var obj in Interactable.all)
        {
            if (!obj.canInteract) continue;

            Vector3 toObject = obj.ClosestPoint(position) - position;
            toObject.y = 0.0f;

            float distance = toObject.magnitude;
            if (distance > interactRange) continue;

            // Touching (or inside) it counts as facing it
            float angle = (distance > 0.001f) ? (Vector3.Angle(forward, toObject)) : (0.0f);
            if (angle > interactAngle) continue;

            if (angle < bestAngle)
            {
                best = obj;
                bestAngle = angle;
            }
        }

        return best;
    }
}
