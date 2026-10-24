using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Pauses the action list. Only does anything with "wait" on, which is how it's created.
[Serializable]
[GameActionName("Flow/Wait")]
public class GameAction_Wait : GameAction
{
    [SerializeField, Min(0), Tooltip("Seconds")]
    private float   duration = 0.5f;

    public GameAction_Wait()
    {
        wait = true;
    }

    public override IEnumerator Execute(ActionContext context)
    {
        yield return new WaitForSeconds(duration);
    }
}
