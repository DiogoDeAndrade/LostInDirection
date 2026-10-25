using UnityEngine;

// Keeps a character turned towards something (the actor watching the girl walk off in a cutscene): it turns at
// turnSpeed degrees a second, through its Rigidbody when it has one. Watch() starts it, Stop() ends it; started
// during a cutscene, it also ends with it. Set through the Character/Watch action (which adds one if missing).
public class Watcher : MonoBehaviour
{
    Transform   target;
    float       turnSpeed;
    bool        untilCutsceneEnds;
    Rigidbody   body;

    public bool isWatching => (target != null);

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Watch(Transform target, float turnSpeed)
    {
        this.target = target;
        this.turnSpeed = turnSpeed;
        untilCutsceneEnds = Cutscene.isActive;
    }

    public void Stop()
    {
        target = null;
    }

    void Update()
    {
        if (body == null) Turn(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (body != null) Turn(Time.fixedDeltaTime);
    }

    void Turn(float deltaTime)
    {
        if (target == null) return;
        if ((untilCutsceneEnds) && (!Cutscene.isActive))
        {
            Stop();
            return;
        }

        Vector3 d = target.position - transform.position;
        d.y = 0.0f;
        if (d.sqrMagnitude < 0.0001f) return;

        var current = (body != null) ? (body.rotation) : (transform.rotation);
        var r = Quaternion.RotateTowards(current, Quaternion.LookRotation(d, Vector3.up), turnSpeed * deltaTime);
        if (body != null) body.MoveRotation(r); else transform.rotation = r;
    }
}
