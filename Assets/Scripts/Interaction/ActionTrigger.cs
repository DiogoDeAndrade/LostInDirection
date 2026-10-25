using UC.Interaction;
using UnityEngine;

// A zone (a trigger collider on this object) that runs a list of actions when the actor is in it and its conditions
// hold (checked while he's inside, so they can come true then): the same conditions and actions verbs use, with the
// actor as the context's trigger object and this object as its target. The actions run on the actor. Not while
// gameplay is on hold. Fires once, unless once is off (then again after he leaves and comes back).
[RequireComponent(typeof(Collider))]
public class ActionTrigger : MonoBehaviour
{
    [SerializeField]
    private bool            once = true;
    [SerializeReference]
    private Condition[]     conditions;
    [SerializeReference]
    private GameAction[]    actions;

    bool    fired;

    void OnTriggerStay(Collider other)
    {
        if ((fired) || (Gameplay.isHeld)) return;

        var actor = other.GetComponentInParent<Player>();
        if (actor == null) return;

        var context = new ActionContext
        {
            triggerGameObject = actor.gameObject,
            targetGameObject = gameObject,
            runner = actor
        };
        if (conditions != null)
        {
            foreach (var condition in conditions)
            {
                if ((condition != null) && (!condition.Evaluate(context))) return;
            }
        }

        fired = true;
        if (actions != null) GameAction.RunActions(actions, context);
    }

    void OnTriggerExit(Collider other)
    {
        if ((!once) && (other.GetComponentInParent<Player>() != null)) fired = false;
    }
}
