using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Switches a zombie's follow mode on (it starts walking after the actor) or off
[Serializable]
[GameActionName("Zombie/Follow")]
public class GameAction_ZombieFollow : GameAction
{
    [SerializeField, Tooltip("The zombie (spawned ones are easiest to find by hypertag)")]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Hypertag);
    [SerializeField]
    private bool            follow = true;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        var zombie = (obj != null) ? (obj.GetComponentInParent<Zombie>()) : (null);
        if (zombie == null)
        {
            Debug.LogWarning($"Zombie Follow: no zombie ({target})");
            yield break;
        }

        zombie.SetFollow(follow);
    }
}
