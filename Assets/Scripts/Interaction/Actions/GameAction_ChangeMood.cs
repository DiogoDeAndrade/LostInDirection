using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Changes the director's mood (the actor's health): up for right actions, down for wrong ones. The director has 8
// faces, so one face is 1/7 (about 0.14) of the mood.
[Serializable]
[GameActionName("Director/Change Mood")]
public class GameAction_ChangeMood : GameAction
{
    [SerializeField, Range(-1.0f, 1.0f), Tooltip("Added to the mood (0 furious, 1 delighted); -0.07 is half a face")]
    private float   amount = -0.07f;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        if (DirectorDriver.instance == null)
        {
            Debug.LogWarning("Change Mood: no DirectorDriver in the scene");
            yield break;
        }

        DirectorDriver.instance.ChangeMood(amount);
    }
}
