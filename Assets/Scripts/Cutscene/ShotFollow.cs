using UnityEngine;

// Keeps a camera on a subject for a cutscene: it eases to the subject's position plus an offset (in world space) and
// looks at the subject plus a look offset, both smoothed, so a new subject or offset (the next shot) is a smooth
// move too, starting from wherever the camera is looking. Follow() starts it, Stop() leaves the camera where it is;
// switching the camera off stops it. Set through the Camera/Follow action (which adds one if the camera has none).
public class ShotFollow : MonoBehaviour
{
    Transform   subject;
    Vector3     offset;
    Vector3     lookOffset;
    float       smoothTime;
    Vector3     lookPoint;
    Vector3     velocity;
    Vector3     lookVelocity;

    public bool isFollowing => (subject != null);

    public void Follow(Transform subject, Vector3 offset, Vector3 lookOffset, float smoothTime)
    {
        if (this.subject == null)
        {
            // Start from what the camera is looking at now, as far ahead as the new look point
            float distance = Vector3.Distance(transform.position, subject.position + lookOffset);
            lookPoint = transform.position + transform.forward * distance;
            velocity = lookVelocity = Vector3.zero;
        }
        this.subject = subject;
        this.offset = offset;
        this.lookOffset = lookOffset;
        this.smoothTime = smoothTime;
    }

    public void Stop()
    {
        subject = null;
    }

    void OnDisable()
    {
        Stop();
    }

    void LateUpdate()
    {
        if (subject == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, subject.position + offset, ref velocity, smoothTime);
        lookPoint = Vector3.SmoothDamp(lookPoint, subject.position + lookOffset, ref lookVelocity, smoothTime);
        var dir = lookPoint - transform.position;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir);
    }
}
