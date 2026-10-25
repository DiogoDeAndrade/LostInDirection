using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Has a camera (a cutscene camera) follow a subject, from the subject's position plus offset (world space), looking
// at it plus lookOffset, easing in over about smoothTime; on a camera already following, it moves on to the new
// subject or shot the same way. Off, the camera stays where it is. Through the camera's ShotFollow (added if missing).
[Serializable]
[GameActionName("Camera/Follow")]
public class GameAction_CameraFollow : GameAction
{
    [SerializeField]
    private ObjectTarget    camera = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField]
    private bool            follow = true;
    [SerializeField]
    private ObjectTarget    subject = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField, Tooltip("Where the camera goes, from the subject (world space)")]
    private Vector3         offset = new Vector3(0.0f, 1.7f, -2.6f);
    [SerializeField, Tooltip("What it looks at, from the subject (world space)")]
    private Vector3         lookOffset = new Vector3(0.0f, 0.45f, 0.0f);
    [SerializeField, Min(0.01f), Tooltip("Seconds, roughly, it takes to catch up")]
    private float           smoothTime = 0.8f;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = camera.GetGameObject(context);
        if (obj == null)
        {
            Debug.LogWarning($"Camera Follow: no camera ({camera})");
            yield break;
        }

        var shot = obj.GetComponent<ShotFollow>();
        if (!follow)
        {
            if (shot != null) shot.Stop();
            yield break;
        }

        var target = subject.GetGameObject(context);
        if (target == null)
        {
            Debug.LogWarning($"Camera Follow: nothing to follow ({subject})");
            yield break;
        }

        if (shot == null) shot = obj.AddComponent<ShotFollow>();
        shot.Follow(target.transform, offset, lookOffset, smoothTime);
    }
}
