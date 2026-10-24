using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// The director talks (the briefing, ...): the lines are what he means, shown one after another in his speech balloon
// as gibberish (unless translate is off; {braces} keep their text). Created with "wait" on, so the next action starts
// when he's done. While he talks the actor has no control and frustration and mood don't change.
[Serializable]
[GameActionName("Director/Says")]
public class GameAction_DirectorSays : GameAction
{
    [SerializeField, TextArea(1, 4), Tooltip("What he means, one balloon per line")]
    private string[]    lines;
    [SerializeField, Tooltip("Show the lines as gibberish (off: as written, for testing)")]
    private bool        translate = true;
    [SerializeField, Min(0), Tooltip("Seconds to wait for the Director scene to load before he starts")]
    private float       maxWaitForDirector = 3.0f;

    public GameAction_DirectorSays()
    {
        wait = true;
    }

    public override IEnumerator Execute(ActionContext context)
    {
        if (DirectorSpeech.instance == null)
        {
            Debug.LogWarning("Director Says: no DirectorSpeech in the scene");
            yield break;
        }

        // At the start of a movie the Director scene may still be loading
        float waited = 0.0f;
        while ((Director.instance == null) && (waited < maxWaitForDirector))
        {
            waited += Time.deltaTime;
            yield return null;
        }

        yield return DirectorSpeech.instance.Say(lines, translate);
    }
}
