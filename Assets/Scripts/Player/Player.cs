using NaughtyAttributes;
using System.Collections.Generic;
using UC;
using UC.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

// The actor (its animation is a CharacterAnimation next to it).
// Interaction: the closest-to-facing Interactable in range with an available verb is the target (outlined, with
// the prompt next to it); one button cycles its available verbs, another performs the selected one. Verb actions
// run on the actor (it's their ActionRunner). While gameplay is on hold (cutscene, director talking, slate) the
// controls are locked: no movement, no aiming, no interaction.
[RequireComponent(typeof(MovementDirectionalXZ))]
public class Player : ActionRunner
{
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
    [SerializeField]
    private Hypertag            uiInventoryDisplay;

    MovementDirectionalXZ movement;
    Interactable            target;
    Verb                    selectedVerb;
    List<Verb>              verbs = new List<Verb>();
    List<Verb>              candidateVerbs = new List<Verb>();
    ActionContext           context = new ActionContext();

    public bool needNewInputSystem => (interactInput != null) && (interactInput.type == UC.InputControl.InputType.NewInput);

    // What the actor is facing and can interact with (null when nothing is)
    public Interactable currentTarget => target;

    // Movement, aiming and interaction switched off (cutscenes)
    public bool controlsLocked { get; private set; }

    public void LockControls(bool locked)
    {
        if (controlsLocked == locked) return;
        controlsLocked = locked;

        movement.SetActive(!locked);
        if (locked) SetTarget(null);
    }

    void Start()
    {
        movement = GetComponent<MovementDirectionalXZ>();
        if (prompt == null) prompt = FindAnyObjectByType<InteractionPrompt>();

        if (interactInput != null) interactInput.playerInput = playerInput;
        if (nextVerbInput != null) nextVerbInput.playerInput = playerInput;

        // Used to check verb conditions; the target is filled in by each interactable
        context.triggerGameObject = gameObject;
        context.runner = this;

        // 
        var inventoryDisplay = uiInventoryDisplay.FindFirst<InventoryDisplay>();
        inventoryDisplay.SetInventory(GetComponent<Inventory>());
    }

    void Update()
    {
        LockControls(Gameplay.isHeld);
        UpdateInteraction();
    }

    void SetTarget(Interactable newTarget)
    {
        // An action may have destroyed the target
        if (!target) target = null;
        if (newTarget == target) return;

        if (target) target.SetHighlight(false);
        target = newTarget;
        selectedVerb = null;
        if (target) target.SetHighlight(true);
    }

    void UpdateInteraction()
    {
        SetTarget((controlsLocked) ? (null) : (FindTarget()));

        // Conditions can change while the target stays the same, so the list is rebuilt every frame; the selection
        // follows the verb, and goes back to the first one if that verb is gone
        verbs.Clear();
        if (target != null) target.GetAvailableVerbs(context, verbs);

        int verbIndex = verbs.IndexOf(selectedVerb);
        if (verbIndex < 0)
        {
            verbIndex = 0;
            selectedVerb = (verbs.Count > 0) ? (verbs[0]) : (null);
        }

        if (selectedVerb != null)
        {
            if (nextVerbInput.IsDown())
            {
                verbIndex = (verbIndex + 1) % verbs.Count;
                selectedVerb = verbs[verbIndex];
            }
            if (interactInput.IsDown()) target.Interact(gameObject, this, selectedVerb);
        }

        if (prompt != null)
        {
            bool gamepad = (playerInput != null) && (playerInput.currentControlScheme == "Gamepad");
            prompt.Show(target, verbs, verbIndex, gamepad);
        }
    }

    // The interactable in range, with something available to do, that is closest to straight ahead
    Interactable FindTarget()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;
        forward.y = 0.0f;

        Interactable best = null;
        float bestAngle = float.MaxValue;

        foreach (var obj in Interactable.all)
        {
            Vector3 toObject = obj.ClosestPoint(position) - position;
            toObject.y = 0.0f;

            float distance = toObject.magnitude;
            if (distance > interactRange) continue;

            // Touching (or inside) it counts as facing it
            float angle = (distance > 0.001f) ? (Vector3.Angle(forward, toObject)) : (0.0f);
            if (angle > interactAngle) continue;
            if (angle >= bestAngle) continue;

            // Nothing to do with it right now: it doesn't count as interactable
            obj.GetAvailableVerbs(context, candidateVerbs);
            if (candidateVerbs.Count == 0) continue;

            best = obj;
            bestAngle = angle;
        }

        return best;
    }
}
