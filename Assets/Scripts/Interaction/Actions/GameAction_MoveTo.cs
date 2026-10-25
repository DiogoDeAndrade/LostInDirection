using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Moves an object to another's position (and rotation), easing in and out over time seconds: a cutscene camera
// pulling back from one shot to the next, ... With "wait", the next action starts when it's there.
[Serializable]
[GameActionName("Object/Move To")]
public class GameAction_MoveTo : GameAction
{
    [SerializeField]
    private ObjectTarget    what = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField, Tooltip("Where to (its position and rotation)")]
    private ObjectTarget    to = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField, Min(0.0f)]
    private float           time = 1.5f;
    [SerializeField]
    private bool            rotate = true;

    public override IEnumerator Execute(ActionContext context)
    {
        var a = what.GetGameObject(context);
        var b = to.GetGameObject(context);
        if ((a == null) || (b == null))
        {
            Debug.LogWarning($"Move To: nothing to move ({what}) or nowhere to go ({to})");
            yield break;
        }

        var t = a.transform;
        Vector3 fromPos = t.position;
        Quaternion fromRot = t.rotation;
        for (float e = 0.0f; e < time; e += Time.deltaTime)
        {
            if ((t == null) || (b == null)) yield break;
            float k = Mathf.SmoothStep(0.0f, 1.0f, e / time);
            t.position = Vector3.Lerp(fromPos, b.transform.position, k);
            if (rotate) t.rotation = Quaternion.Slerp(fromRot, b.transform.rotation, k);
            yield return null;
        }
        t.position = b.transform.position;
        if (rotate) t.rotation = b.transform.rotation;
    }
}
