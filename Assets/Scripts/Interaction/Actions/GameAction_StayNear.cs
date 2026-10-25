using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Instead of a target, the director wants the actor to stay within radius of an Interactable, or of a place (the
// first object with a hypertag): his frustration goes down while he's there and up while he's away (see
// DirectorDriver). Set Player Target ends it.
[Serializable]
[GameActionName("Director/Stay Near")]
public class GameAction_StayNear : GameAction
{
    [SerializeField]
    private Interactable    interactable;
    [SerializeField, Tooltip("Or a place: the first object with this hypertag (used when there's no Interactable)")]
    private Hypertag        hypertag;
    [SerializeField, Min(0.0f)]
    private float           radius = 3.0f;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        if (DirectorDriver.instance == null)
        {
            Debug.LogWarning("Stay Near: no DirectorDriver in the scene");
            yield break;
        }

        Transform place = (interactable != null) ? (interactable.transform) : ((hypertag != null) ? (Hypertag.FindFirstObjectWithHypertag<Transform>(hypertag)) : (null));
        if (place == null)
        {
            Debug.LogWarning("Stay Near: nowhere to stay near");
            yield break;
        }

        DirectorDriver.instance.SetStayNear(place, radius);
    }
}
