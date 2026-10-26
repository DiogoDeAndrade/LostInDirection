using NaughtyAttributes;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;

// The actor's bow. Armed (by the Player/Bow Mode action), the aim input (right stick) turns him even while he walks,
// and the shoot input (either trigger) looses an arrow along his facing, at most every cooldown seconds, with the
// shooting animation. The bow shows held out in front of him (holdPosition / holdRotation, from his root) while he
// aims and for showAfterShot after each shot, and is hidden otherwise. Nothing while gameplay is on hold. Sounds:
// drawSound as he starts aiming, shootSound with each arrow.
[RequireComponent(typeof(MovementDirectionalXZ))]
public class Archer : MonoBehaviour
{
    [SerializeField, ShowIf(nameof(needNewInputSystem))]
    private PlayerInput     playerInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    private UC.InputControl shootInput;
    [SerializeField, Tooltip("The bow he holds, shown while he aims")]
    private GameObject      heldBow;
    [SerializeField, Tooltip("Where the bow is held, from his feet, along his facing")]
    private Vector3         holdPosition = new Vector3(0.0f, 0.16f, 0.24f);
    [SerializeField, Tooltip("Its rotation there (identity: limbs up, bulging forward)")]
    private Vector3         holdRotation = Vector3.zero;
    [SerializeField, Min(0.0f), Tooltip("Seconds the bow stays out after a shot, when he isn't aiming")]
    private float           showAfterShot = 0.5f;
    [SerializeField]
    private Arrow           arrowPrefab;
    [SerializeField, Tooltip("Where arrows start, from his feet, along his facing")]
    private Vector3         muzzleOffset = new Vector3(0.0f, 0.35f, 0.35f);
    [SerializeField, Min(0.05f)]
    private float           cooldown = 0.45f;
    [SerializeField]
    private AnimationClip   shootClip;
    [SerializeField]
    private SoundDef        drawSound;
    [SerializeField]
    private SoundDef        shootSound;

    public bool armed { get; private set; }

    public bool needNewInputSystem => (shootInput != null) && (shootInput.type == UC.InputControl.InputType.NewInput);

    MovementDirectionalXZ   movement;
    CharacterAnimation      characterAnimation;
    float                   cooldownTimer;
    float                   sinceShot = float.MaxValue;
    bool                    wasAiming;

    void Start()
    {
        movement = GetComponent<MovementDirectionalXZ>();
        characterAnimation = GetComponent<CharacterAnimation>();
        if (shootInput != null) shootInput.playerInput = playerInput;
        if (heldBow != null)
        {
            heldBow.transform.SetParent(transform, false);
            heldBow.transform.localPosition = holdPosition;
            heldBow.transform.localRotation = Quaternion.Euler(holdRotation);
        }
        SetArmed(false);
    }

    public void SetArmed(bool value)
    {
        armed = value;
        if (movement != null) movement.aimWhileMoving = armed;
        ShowBow(false);
    }

    void Update()
    {
        if (cooldownTimer > 0.0f) cooldownTimer -= Time.deltaTime;
        sinceShot += Time.deltaTime;
        bool held = Gameplay.isHeld;
        bool aiming = armed && !held && (movement != null) && (movement.isAiming);
        if ((aiming) && (!wasAiming) && (drawSound != null)) drawSound.Play();
        wasAiming = aiming;
        ShowBow(armed && !held && ((aiming) || (sinceShot < showAfterShot)));

        if ((!armed) || (held) || (cooldownTimer > 0.0f) || (shootInput == null) || (!shootInput.IsDown())) return;

        cooldownTimer = cooldown;
        sinceShot = 0.0f;
        ShowBow(true);
        Vector3 forward = transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        var rotation = Quaternion.LookRotation(forward, Vector3.up);
        var arrow = Instantiate(arrowPrefab, transform.position + rotation * muzzleOffset, rotation);
        arrow.Launch(gameObject);
        if (shootSound != null) shootSound.Play();
        if ((characterAnimation != null) && (shootClip != null)) characterAnimation.PlayOneShot(shootClip, 0.05f);
    }

    void ShowBow(bool show)
    {
        if ((heldBow != null) && (heldBow.activeSelf != show)) heldBow.SetActive(show);
    }
}
