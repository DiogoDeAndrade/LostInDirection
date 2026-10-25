using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// A character turns to face an object, over a moment (the actor facing whoever comes up to him, ...)
[Serializable]
[GameActionName("Character/Face")]
public class GameAction_Face : GameAction
{
    [SerializeField, Tooltip("Who turns")]
    private ObjectTarget    who = new ObjectTarget(ObjectTarget.Mode.Actor);
    [SerializeField, Tooltip("Towards what")]
    private ObjectTarget    toward = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField, Min(0.0f)]
    private float           time = 0.2f;

    public override IEnumerator Execute(ActionContext context)
    {
        var a = who.GetGameObject(context);
        var b = toward.GetGameObject(context);
        if ((a == null) || (b == null))
        {
            Debug.LogWarning($"Face: no one to turn ({who}) or nothing to face ({toward})");
            yield break;
        }

        Vector3 d = b.transform.position - a.transform.position;
        d.y = 0.0f;
        if (d.sqrMagnitude > 0.0001f) yield return Turn(a.transform, Quaternion.LookRotation(d, Vector3.up), time);
    }

    // Turns a transform (through its Rigidbody, when it has one) to a rotation over time seconds
    public static IEnumerator Turn(Transform t, Quaternion to, float time)
    {
        var body = t.GetComponent<Rigidbody>();
        Quaternion from = t.rotation;
        for (float e = 0.0f; e < time; e += Time.deltaTime)
        {
            var r = Quaternion.Slerp(from, to, e / time);
            if (body != null) body.MoveRotation(r); else t.rotation = r;
            yield return null;
        }
        if (body != null) body.MoveRotation(to);
        t.rotation = to;
    }
}
