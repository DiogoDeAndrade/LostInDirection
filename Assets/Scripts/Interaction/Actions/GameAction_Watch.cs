using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// A character keeps facing an object as it moves (the actor watching the girl go), turning at turnSpeed degrees a
// second, until switched off or, when started in a cutscene, until it ends. Through the character's Watcher (added
// if missing).
[Serializable]
[GameActionName("Character/Watch")]
public class GameAction_Watch : GameAction
{
    [SerializeField, Tooltip("Who watches")]
    private ObjectTarget    who = new ObjectTarget(ObjectTarget.Mode.Actor);
    [SerializeField]
    private bool            watch = true;
    [SerializeField, Tooltip("What it watches")]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField, Min(1.0f), Tooltip("Degrees a second")]
    private float           turnSpeed = 240.0f;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var a = who.GetGameObject(context);
        if (a == null)
        {
            Debug.LogWarning($"Watch: no one to watch ({who})");
            yield break;
        }

        var watcher = a.GetComponent<Watcher>();
        if (!watch)
        {
            if (watcher != null) watcher.Stop();
            yield break;
        }

        var b = target.GetGameObject(context);
        if (b == null)
        {
            Debug.LogWarning($"Watch: nothing to watch ({target})");
            yield break;
        }

        if (watcher == null) watcher = a.AddComponent<Watcher>();
        watcher.Watch(b.transform, turnSpeed);
    }
}
