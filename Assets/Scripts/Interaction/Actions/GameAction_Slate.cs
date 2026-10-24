using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// The slate comes into the director camera's view, claps and goes (the take starts). Created with "wait" on, so the
// next action starts when it's gone; gameplay is on hold while it plays.
[Serializable]
[GameActionName("Director/Slate")]
public class GameAction_Slate : GameAction
{
    [SerializeField, Min(0), Tooltip("Seconds to wait for the Director scene to load before it starts")]
    private float   maxWaitForSlate = 3.0f;

    public GameAction_Slate()
    {
        wait = true;
    }

    public override IEnumerator Execute(ActionContext context)
    {
        float waited = 0.0f;
        while ((Slate.instance == null) && (waited < maxWaitForSlate))
        {
            waited += Time.deltaTime;
            yield return null;
        }
        if (Slate.instance == null)
        {
            Debug.LogWarning("Slate: no Slate in the Director scene");
            yield break;
        }

        yield return Slate.instance.Play();
    }
}
