using UC;
using UnityEngine;
using UnityEngine.AI;

// A zombie. Stands still until follow mode is switched on (an action), then walks after the actor on the NavMesh.
[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour
{
    [SerializeField, Tooltip("What it follows")]
    private Hypertag    targetTag;
    [SerializeField]
    private bool        followOnStart = false;
    [SerializeField, Min(0.05f), Tooltip("Seconds between path updates while following")]
    private float       repathInterval = 0.25f;

    NavMeshAgent    agent;
    Transform       target;
    bool            following;
    float           repathTimer;

    public bool isFollowing => following;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        SetFollow(followOnStart);
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
