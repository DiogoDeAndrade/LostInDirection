using NaughtyAttributes;
using System.Collections.Generic;
using UC;
using UnityEngine;
using UnityEngine.AI;

// A zombie. Stands still until follow mode is switched on (an action), then walks after the actor on the NavMesh,
// stopping where it is while gameplay is on hold. It grunts (groanSound, from where it is) when it appears, and now
// and then while it follows.
[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour
{
    [SerializeField, Tooltip("What it follows")]
    private Hypertag    targetTag;
    [SerializeField]
    private bool        followOnStart = false;
    [SerializeField, Min(0.05f), Tooltip("Seconds between path updates while following")]
    private float       repathInterval = 0.25f;
    [SerializeField]
    private SoundDef    groanSound;
    [SerializeField, MinMaxSlider(0.5f, 20.0f), Tooltip("Seconds between grunts while following")]
    private Vector2     groanInterval = new Vector2(4.0f, 9.0f);

    static List<Zombie> _all = new List<Zombie>();

    // Every enabled zombie in the scene
    public static IReadOnlyList<Zombie> all => _all;

    NavMeshAgent    agent;
    Transform       target;
    bool            following;
    float           repathTimer;
    float           groanTimer;

    public bool isFollowing => following;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        _all.Add(this);
    }

    void OnDisable()
    {
        _all.Remove(this);
    }

    void Start()
    {
        SetFollow(followOnStart);
        Groan();
    }

    void Groan()
    {
        if (groanSound != null) groanSound.Play(1.0f, 1.0f, -float.MaxValue, transform.position);
        groanTimer = Random.Range(groanInterval.x, groanInterval.y);
    }

    public void SetFollow(bool follow)
    {
        following = follow;
        repathTimer = 0.0f;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = !follow;
            if (!follow) agent.ResetPath();
        }
    }

    void Update()
    {
        if (!following) return;

        bool held = Gameplay.isHeld;
        if ((agent.isOnNavMesh) && (agent.isStopped != held))
        {
            agent.isStopped = held;
            if (held) agent.velocity = Vector3.zero;
        }
        if (held) return;

        groanTimer -= Time.deltaTime;
        if (groanTimer <= 0.0f) Groan();

        if (target == null)
        {
            target = (targetTag != null) ? (Hypertag.FindFirstObjectWithHypertag<Transform>(targetTag)) : (null);
            if (target == null) return;
        }

        repathTimer -= Time.deltaTime;
        if ((repathTimer <= 0.0f) && (agent.isOnNavMesh))
        {
            agent.SetDestination(target.position);
            repathTimer = repathInterval;
        }
    }
}
