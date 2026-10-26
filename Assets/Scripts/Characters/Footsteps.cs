using UC;
using UnityEngine;

// A character's footsteps, from how far it actually travels over the ground: a step every stride units while it moves
// faster than minSpeed (so none while it turns on the spot or pushes against a wall), the first soon after it sets
// off. Jumps (a teleport, the scene starting) don't count. Stopping means stillTime without moving: a body moved by
// physics only moves on physics steps, so it seems to stand still on the frames in between. The sound is per scene,
// for its ground (grass, stone, ...).
public class Footsteps : MonoBehaviour
{
    [SerializeField]
    private SoundDef    sound;
    [SerializeField, Min(0.05f), Tooltip("Ground distance between steps")]
    private float       stride = 0.65f;
    [SerializeField, Min(0.0f), Tooltip("Slower than this (units/s) isn't walking")]
    private float       minSpeed = 0.3f;
    [SerializeField, Min(0.0f), Tooltip("Seconds not walking before it counts as having stopped")]
    private float       stillTime = 0.2f;

    Vector3     lastPosition;
    float       travelled;
    float       still;

    void OnEnable()
    {
        lastPosition = transform.position;
        travelled = stride * 0.5f;
    }

    void Update()
    {
        Vector3 d = transform.position - lastPosition;
        d.y = 0.0f;
        lastPosition = transform.position;

        float dt = Time.deltaTime;
        if (dt <= 0.0f) return;

        float distance = d.magnitude;
        if (distance > 1.0f)
        {
            // Moved in one go: the first step of the next walk comes half a stride in
            travelled = stride * 0.5f;
            return;
        }
        if (distance / dt < minSpeed)
        {
            still += dt;
            if (still >= stillTime) travelled = stride * 0.5f;
            return;
        }

        still = 0.0f;
        travelled += distance;
        if (travelled >= stride)
        {
            travelled -= stride;
            if (sound != null) sound.Play();
        }
    }
}
