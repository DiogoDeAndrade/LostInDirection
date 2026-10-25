using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Switches an object on or off (a door's blocker once it's open, its lock's interactable once it's used, ...)
[Serializable]
[GameActionName("Object/Set Active")]
public class GameAction_SetActive : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField]
    private bool            active;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        if (obj == null)
        {
            Debug.LogWarning($"Set Active: no object ({target})");
            yield break;
        }

        obj.SetActive(active);
    }
}
