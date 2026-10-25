using UnityEngine;

// A coin to collect: it spins and bobs where it is, and when the actor walks into it, it pops (swells and shrinks
// away) and is gone. Nothing else; it's there so exploring feels rewarded.
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
        if (toActor.sqrMagnitude < pickupRadius * pickupRadius) popElapsed = 0.0f;
    }
}
