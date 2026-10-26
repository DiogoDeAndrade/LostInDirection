using UC;
using UnityEngine;

// An arrow in flight: straight along its forward at speed, sweeping a small sphere ahead each frame. The first orc it
// meets dies; anything else solid stops it, and it stays stuck there for a moment. It's gone after maxDistance.
// Triggers and the shooter's own colliders don't count. The first sweep starts inside the shooter (level with the
// arrow), and an orc the sweep starts inside still counts, so one right up against him is hit too. Sticking in
// something plays wallSound there.
public class Arrow : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    private float   speed = 14.0f;
    [SerializeField, Min(0.1f)]
    private float   maxDistance = 14.0f;
    [SerializeField, Min(0.0f)]
    private float   radius = 0.08f;
    [SerializeField, Min(0.0f), Tooltip("Seconds it stays stuck in a wall")]
    private float   stuckTime = 1.0f;
    [SerializeField]
    private SoundDef    wallSound;

    GameObject  owner;
    float       travelled;
    bool        stuck;
    float       backtrack;

    public void Launch(GameObject shooter)
    {
        owner = shooter;
        backtrack = Mathf.Max(0.0f, Vector3.Dot(transform.position - shooter.transform.position, transform.forward));
    }

    void Update()
    {
        if (stuck) return;

        float step = speed * Time.deltaTime;
        var origin = transform.position - transform.forward * backtrack;
        var hits = Physics.SphereCastAll(origin, radius, transform.forward, step + backtrack, ~0, QueryTriggerInteraction.Ignore);
        RaycastHit? nearest = null;
        foreach (var hit in hits)
        {
            if ((owner != null) && (hit.collider.transform.IsChildOf(owner.transform))) continue;
            if ((hit.distance == 0.0f) && (hit.point == Vector3.zero))
            {
                // Already inside it where the sweep starts: only a live orc counts
                var inside = hit.collider.GetComponentInParent<Orc>();
                if ((inside == null) || (inside.isDead)) continue;
            }
            if ((nearest == null) || (hit.distance < nearest.Value.distance)) nearest = hit;
        }

        if (nearest != null)
        {
            var orc = nearest.Value.collider.GetComponentInParent<Orc>();
            if ((orc != null) && (!orc.isDead))
            {
                orc.Hit(transform.forward);
                Destroy(gameObject);
                return;
            }
            if (orc == null)
            {
                transform.position += transform.forward * (nearest.Value.distance - backtrack);
                stuck = true;
                if (wallSound != null) wallSound.Play(1.0f, 1.0f, -float.MaxValue, transform.position);
                Destroy(gameObject, stuckTime);
                return;
            }
        }

        transform.position += transform.forward * step;
        travelled += step;
        backtrack = 0.0f;
        if (travelled >= maxDistance) Destroy(gameObject);
    }
}
