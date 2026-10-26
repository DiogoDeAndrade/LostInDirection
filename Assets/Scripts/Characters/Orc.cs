using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UC;
using UnityEngine;
using UnityEngine.AI;

// An orc: walks after the actor on the NavMesh and, within attackRange, swings at him (attackClip): if he's still in
// reach when the blow lands (attackWindup in), the director's mood drops by hitMoodCost, the screen flashes red and
// "Ouch!" floats over him; then it waits attackCooldown. One arrow (Hit) kills it: blood sprays out along the arrow's
// line (bloodSplash) and splats the floor behind it (bloodSplats of bloodDecal), it flashes hitColor, is knocked back
// along the line, falls (dieClip) and a pool spreads under it while it lies there corpseTime, then it sinks into the
// floor and is gone; the director's mood goes up by killMoodReward. It stands still while gameplay is on hold.
// Sounds, from where it is: spawnSound as it appears, attackSound as it swings, hitSound and deathSound when an arrow
// gets it.
[RequireComponent(typeof(NavMeshAgent))]
public class Orc : MonoBehaviour
{
    [SerializeField, Tooltip("What it goes after")]
    private Hypertag        targetTag;
    [SerializeField, Min(0.05f), Tooltip("Seconds between path updates")]
    private float           repathInterval = 0.25f;
    [SerializeField, Min(0.0f), Tooltip("Ground distance (centre to centre) it swings from")]
    private float           attackRange = 0.55f;
    [SerializeField, Min(0.0f)]
    private float           attackWindup = 0.3f;
    [SerializeField, Min(0.0f)]
    private float           attackCooldown = 1.3f;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("A face is 1/7 of the mood")]
    private float           hitMoodCost = 1.0f / 7.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float           killMoodReward = 0.07f;
    [SerializeField]
    private AnimationClip   attackClip;
    [SerializeField]
    private AnimationClip   dieClip;
    [SerializeField, Min(0.0f), Tooltip("Seconds lying there before it sinks away")]
    private float           corpseTime = 1.5f;
    [SerializeField]
    private Color           hitColor = new Color(1.0f, 0.25f, 0.2f);
    [SerializeField, Min(0.01f), Tooltip("Seconds the hit colour takes to fade")]
    private float           flashTime = 0.4f;
    [SerializeField, Min(0.0f), Tooltip("How far the arrow knocks it back")]
    private float           knockback = 0.35f;
    [SerializeField, Tooltip("Spawned where it's hit, facing along the arrow")]
    private ParticleSystem  bloodSplash;
    [SerializeField, Tooltip("Height of the hit, from its feet")]
    private float           bloodHeight = 0.45f;
    [SerializeField]
    private BloodDecal      bloodDecal;
    [SerializeField, MinMaxSlider(0, 8), Tooltip("Splats on the floor behind it")]
    private Vector2Int      bloodSplats = new Vector2Int(2, 4);
    [SerializeField, MinMaxSlider(0.05f, 1.0f)]
    private Vector2         splatSize = new Vector2(0.15f, 0.35f);
    [SerializeField, Min(0.0f), Tooltip("The pool under the body (0: none)")]
    private float           poolSize = 0.6f;
    [SerializeField, Tooltip("Each splat landing")]
    private SoundDef        splatSound;
    [SerializeField]
    private SoundDef        spawnSound;
    [SerializeField]
    private SoundDef        attackSound;
    [SerializeField, Tooltip("The arrow going in")]
    private SoundDef        hitSound;
    [SerializeField]
    private SoundDef        deathSound;

    static List<Orc> _all = new List<Orc>();

    // Every orc in the scene, dead ones until they're gone
    public static IReadOnlyList<Orc> all => _all;

    public bool isDead { get; private set; }

    NavMeshAgent        agent;
    CharacterAnimation  characterAnimation;
    Transform           target;
    float               repathTimer;
    float               attackTimer;
    bool                attacking;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        characterAnimation = GetComponent<CharacterAnimation>();
    }

    void Start()
    {
        PlayHere(spawnSound);
    }

    void PlayHere(SoundDef sound)
    {
        if (sound != null) sound.Play(1.0f, 1.0f, -float.MaxValue, transform.position + Vector3.up * 0.5f);
    }

    void OnEnable() { _all.Add(this); }
    void OnDisable() { _all.Remove(this); }

    void Update()
    {
        if (isDead) return;

        bool held = Gameplay.isHeld;
        if ((agent.isOnNavMesh) && (agent.isStopped != (held || attacking)))
        {
            agent.isStopped = held || attacking;
            if (agent.isStopped) agent.velocity = Vector3.zero;
        }
        if (held) return;

        if (target == null)
        {
            target = (targetTag != null) ? (Hypertag.FindFirstObjectWithHypertag<Transform>(targetTag)) : (null);
            if (target == null) return;
        }

        if (attackTimer > 0.0f) attackTimer -= Time.deltaTime;
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0.0f;
        if ((!attacking) && (attackTimer <= 0.0f) && (toTarget.magnitude < attackRange))
        {
            StartCoroutine(AttackCR());
            return;
        }

        repathTimer -= Time.deltaTime;
        if ((repathTimer <= 0.0f) && (agent.isOnNavMesh) && (!attacking))
        {
            agent.SetDestination(target.position);
            repathTimer = repathInterval;
        }
    }

    IEnumerator AttackCR()
    {
        attacking = true;
        Face(target.position);
        if ((characterAnimation != null) && (attackClip != null)) characterAnimation.PlayOneShot(attackClip, 0.05f);
        PlayHere(attackSound);
        yield return new WaitForSeconds(attackWindup);

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0.0f;
        if ((!isDead) && (!Gameplay.isHeld) && (toTarget.magnitude < attackRange + 0.2f))
        {
            if (DirectorDriver.instance != null) DirectorDriver.instance.ChangeMood(-hitMoodCost);
            Globals.hurtSnd?.Play();
            ScreenFlash.Flash();
            CombatTextManager.SpawnText(target.gameObject, new Vector2(0.0f, 1.0f), "Ouch!", new CombatTextDef(new Color(1.0f, 0.35f, 0.25f), 1.0f) { followOwner = false });
        }
        attackTimer = attackCooldown;
        attacking = false;
    }

    // Struck by an arrow flying along direction
    public void Hit(Vector3 direction)
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        attacking = false;

        if (agent.isOnNavMesh) agent.isStopped = true;
        agent.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        if (DirectorDriver.instance != null) DirectorDriver.instance.ChangeMood(killMoodReward);
        direction.y = 0.0f;
        if (direction.sqrMagnitude < 0.0001f) direction = -transform.forward;
        direction.Normalize();
        PlayHere(hitSound);
        PlayHere(deathSound);

        if (bloodSplash != null) Instantiate(bloodSplash, transform.position + Vector3.up * bloodHeight, Quaternion.LookRotation(direction));
        StartCoroutine(SplatsCR(transform.position, direction));
        StartCoroutine(FlashCR());
        StartCoroutine(DieCR(direction));
    }

    // Splats behind it along the arrow's line (past where it falls, so the body doesn't cover them), the further ones a
    // moment later (when the drops would land)
    IEnumerator SplatsCR(Vector3 from, Vector3 direction)
    {
        if (bloodDecal == null) yield break;

        int count = Random.Range(bloodSplats.x, bloodSplats.y + 1);
        var side = Vector3.Cross(Vector3.up, direction);
        var splats = new List<(float distance, Vector3 point)>();
        for (int i = 0; i < count; i++)
        {
            float distance = Random.Range(0.6f, 1.6f);
            splats.Add((distance, from + direction * distance + side * Random.Range(-0.35f, 0.35f)));
        }
        splats.Sort((a, b) => a.distance.CompareTo(b.distance));

        float elapsed = 0.0f;
        foreach (var (distance, point) in splats)
        {
            float at = distance * 0.25f;
            if (at > elapsed) { yield return new WaitForSeconds(at - elapsed); elapsed = at; }
            BloodDecal.Spawn(bloodDecal, point, Random.Range(splatSize.x, splatSize.y), 0.08f);
            if (splatSound != null) splatSound.Play(1.0f, 1.0f, -float.MaxValue, point);
        }
    }

    IEnumerator DieCR(Vector3 direction)
    {
        // Facing the shot, so it falls back along it. Straight on the model, with the CharacterAnimation off: it would go
        // back to idle once the clip ended, and the clip ends on the floor
        Face(transform.position - direction);
        if (characterAnimation != null) characterAnimation.enabled = false;
        var handler = GetComponentInChildren<PlayableHandler>();
        float length = 0.0f;
        if ((handler != null) && (dieClip != null))
        {
            handler.Play(dieClip, false, 0.05f);
            length = dieClip.length;
        }

        // Knocked back, quickly and slowing down
        Vector3 start = transform.position;
        const float knockTime = 0.2f;
        for (float t = 0.0f; t < knockTime; t += Time.deltaTime)
        {
            float k = 1.0f - (1.0f - t / knockTime) * (1.0f - t / knockTime);
            transform.position = start + direction * (knockback * k);
            yield return null;
        }
        transform.position = start + direction * knockback;

        // On the floor, the pool spreading from under its chest (it fell back along the arrow's line)
        float lying = Mathf.Max(0.0f, length - knockTime);
        yield return new WaitForSeconds(lying);
        if ((bloodDecal != null) && (poolSize > 0.0f)) BloodDecal.Spawn(bloodDecal, transform.position + direction * 0.25f, poolSize, corpseTime + 1.0f);

        yield return new WaitForSeconds(corpseTime);

        Vector3 from = transform.position;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
        {
            transform.position = from + Vector3.down * (t * 0.8f);
            yield return null;
        }
        Destroy(gameObject);
    }

    // The hit colour over its own, fading out (a property block, so the shared material stays as it is)
    IEnumerator FlashCR()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        var block = new MaterialPropertyBlock();
        for (float t = 0.0f; t < flashTime; t += Time.deltaTime)
        {
            block.SetColor("_BaseColor", Color.Lerp(hitColor, Color.white, t / flashTime));
            foreach (var r in renderers) r.SetPropertyBlock(block);
            yield return null;
        }
        foreach (var r in renderers) r.SetPropertyBlock(null);
    }

    void Face(Vector3 point)
    {
        Vector3 d = point - transform.position;
        d.y = 0.0f;
        if (d.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(d, Vector3.up);
    }
}
