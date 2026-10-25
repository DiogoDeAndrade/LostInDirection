using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Arms or disarms the actor's bow (his Archer): armed, the right stick aims and either trigger shoots
[Serializable]
[GameActionName("Player/Bow Mode")]
public class GameAction_BowMode : GameAction
{
    [SerializeField]
    private bool    armed = true;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var archer = (context.triggerGameObject != null) ? (context.triggerGameObject.GetComponent<Archer>()) : (null);
        if (archer == null)
        {
            Debug.LogWarning("Bow Mode: the actor has no Archer");
            yield break;
        }

        archer.SetArmed(armed);
    }
}
