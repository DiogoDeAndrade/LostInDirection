using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Sets where the actor should go next (the movie's current beat): an Interactable to go to and aim at, or a place
// (the first object with a hypertag). The director's frustration follows how the actor does about it (see
// DirectorDriver). With neither set, it clears the target.
[Serializable]
[GameActionName("Director/Set Player Target")]
public class GameAction_SetPlayerTarget : GameAction
{
    [SerializeField, Tooltip("The object to go to and aim at")]
    private Interactable    interactable;
    [SerializeField, Tooltip("Or a place: the first object with this hypertag (used when there's no Interactable)")]
    private Hypertag        hypertag;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        if (DirectorDriver.instance == null)
        {
            Debug.LogWarning("Set Player Target: no DirectorDriver in the scene");
            yield break;
        }

        Transform place = null;
        if ((interactable == null) && (hypertag != null))
        {
            place = Hypertag.FindFirstObjectWithHypertag<Transform>(hypertag);
            if (place == null) Debug.LogWarning($"Set Player Target: no object with hypertag {hypertag.name}");
        }

        DirectorDriver.instance.SetTarget(interactable, place);
    }
}
