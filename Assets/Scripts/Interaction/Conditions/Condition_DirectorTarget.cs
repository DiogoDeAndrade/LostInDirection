using System;
using UC;
using UC.Interaction;
using UnityEngine;

// The director's current target (the movie's current beat, from Set Player Target) is this Interactable, or this
// place (the first object with the hypertag)
[Serializable]
[ConditionName("Director/Target Is")]
public class Condition_DirectorTarget : Condition
{
    [SerializeField]
    private Interactable    interactable;
    [SerializeField, Tooltip("Or a place: the first object with this hypertag (used when there's no Interactable)")]
    private Hypertag        hypertag;

    protected override bool EvaluateThis(ActionContext context)
    {
        var driver = DirectorDriver.instance;
        if (driver == null) return false;

        if (interactable != null) return driver.target == interactable;
        if (hypertag == null) return false;

        var place = Hypertag.FindFirstObjectWithHypertag<Transform>(hypertag);
        return (place != null) && (driver.target == null) && (driver.targetPlace == place);
    }
}
