using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Starts cutscene mode: bars in, gameplay UI hidden, controls locked, optionally viewed through another camera
// (a disabled one in the scene). With "wait", the next action starts once the bars are in.
[Serializable]
[GameActionName("Cutscene/Start")]
public class GameAction_StartCutscene : GameAction
{
    [SerializeField, Tooltip("Camera to view the cutscene through (it can start disabled); the gameplay camera if empty")]
    private Camera  camera;

    public override IEnumerator Execute(ActionContext context)
    {
        if (Cutscene.instance == null)
        {
            Debug.LogWarning("Start Cutscene: no Cutscene in the scene");
            yield break;
        }

        yield return Cutscene.instance.Begin(camera);
    }
}
