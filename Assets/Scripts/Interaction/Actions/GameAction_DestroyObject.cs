using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Removes an object from the scene
[Serializable]
[GameActionName("Object/Destroy")]
public class GameAction_DestroyObject : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget();

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        if (obj == null)
        {
            Debug.LogWarning($"Destroy: no object to destroy ({target})");
            yield break;
        }

        GameObject.Destroy(obj);
    }
}
