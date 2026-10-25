using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Lowers a force field (it fades out and is gone, collider and all). Created with "wait" on, so the next action starts
// when it's down.
[Serializable]
[GameActionName("FX/Lower Force Field")]
public class GameAction_LowerForceField : GameAction
{
    [SerializeField]
    private ForceField  field;

    public GameAction_LowerForceField()
    {
        wait = true;
    }

    public override IEnumerator Execute(ActionContext context)
    {
        if (field == null)
        {
            Debug.LogWarning("Lower Force Field: no field");
            yield break;
        }

        yield return field.Lower();
    }
}
