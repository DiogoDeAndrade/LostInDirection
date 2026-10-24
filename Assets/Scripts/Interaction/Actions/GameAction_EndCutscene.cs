using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Ends cutscene mode: gameplay camera back, bars out, then UI and controls back. With "wait", the next action
// starts once that's done.
[Serializable]
[GameActionName("Cutscene/End")]
public class GameAction_EndCutscene : GameAction
{
    public override IEnumerator Execute(ActionContext context)
    {
        if (Cutscene.instance == null)
        {
            Debug.LogWarning("End Cutscene: no Cutscene in the scene");
            yield break;
        }

        yield return Cutscene.instance.End();
    }
}
