using UC.Interaction;
using UnityEngine;

// One thing the actor can do with an interactable ("Dig", "Pick up", ...). An object can have several, on itself or
// on its children; its Interactable lists them. A verb is offered only while all its conditions hold, and
// performing it runs its actions in order.
public class Verb : MonoBehaviour
{
    [SerializeField, Tooltip("What the prompt shows")]
    private string          _displayName = "Use";
    [SerializeReference]
    private Condition[]     conditions;
    [SerializeReference]
    private GameAction[]    actions;

    public string displayName => _displayName;

    public bool IsAvailable(ActionContext context)
    {
        if (!isActiveAndEnabled) return false;
        if (conditions == null) return true;

        foreach (var condition in conditions)
        {
            if ((condition != null) && (!condition.Evaluate(context))) return false;
        }

        return true;
    }

    // The actions run on the context's runner (the actor), so they carry on if one of them destroys this object
    public void Perform(ActionContext context)
    {
        if ((actions == null) || (actions.Length == 0)) return;

        GameAction.RunActions(actions, context);
    }
}
