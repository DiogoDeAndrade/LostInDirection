using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// The take is good: the GameManager has the director cheer and moves on to the movie's next scene (see GameManager)
[Serializable]
[GameActionName("Movie/Scene Complete")]
public class GameAction_SceneComplete : GameAction
{
    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("Scene Complete: no GameManager in the scene");
            yield break;
        }

        GameManager.instance.CompleteScene();
    }
}
