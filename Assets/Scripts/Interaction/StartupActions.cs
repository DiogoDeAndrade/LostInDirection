using UC.Interaction;
using UnityEngine;

// Runs a list of actions when the scene starts (the movie's first beat, ...): the same actions verbs use, with the
// actor as the context's trigger object and this object as its target. The actions run on this object.
public class StartupActions : ActionRunner
{
    [SerializeReference]
    private GameAction[]    actions;

    void Start()
    {
        if ((actions == null) || (actions.Length == 0)) return;

        var actor = FindAnyObjectByType<Player>();
        var context = new ActionContext
        {
            triggerGameObject = (actor != null) ? (actor.gameObject) : (null),
            targetGameObject = gameObject,
            runner = this
        };
        GameAction.RunActions(actions, context);
    }
}
