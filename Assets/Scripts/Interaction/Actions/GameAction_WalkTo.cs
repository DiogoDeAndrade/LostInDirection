using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;
using UnityEngine.AI;

// A character with a NavMeshAgent walks to an object (stopping stopDistance short of it) and turns to face it. Created
// with "wait" on, so the next action starts when it's there (or after timeout).
[Serializable]
[GameActionName("Character/Walk To")]
public class GameAction_WalkTo : GameAction
{
    [SerializeField, Tooltip("Who walks (it needs a NavMeshAgent)")]
    private ObjectTarget    walker = new ObjectTarget(ObjectTarget.Mode.Object);
    [SerializeField, Tooltip("Where to")]
    private ObjectTarget    destination = new ObjectTarget(ObjectTarget.Mode.Actor);
    [SerializeField, Min(0.0f)]
    private float           stopDistance = 0.4f;
    [SerializeField, Min(0.1f)]
    private float           timeout = 10.0f;

    public GameAction_WalkTo()
    {
        wait = true;
    }

    public override IEnumerator Execute(ActionContext context)
    {
        var who = walker.GetGameObject(context);
        var where = destination.GetGameObject(context);
        var agent = (who != null) ? (who.GetComponent<NavMeshAgent>()) : (null);
        if ((agent == null) || (where == null))
        {
            Debug.LogWarning($"Walk To: no walker with a NavMeshAgent ({walker}) or no destination ({destination})");
            yield break;
        }

        if (!agent.enabled) agent.enabled = true;
        agent.isStopped = false;
        agent.stoppingDistance = stopDistance;
        agent.SetDestination(where.transform.position);
        for (float t = 0.0f; t < timeout; t += Time.deltaTime)
        {
            yield return null;
            if ((who == null) || (where == null)) yield break;
            if ((!agent.pathPending) && (agent.remainingDistance <= stopDistance + 0.05f)) break;
        }
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 d = where.transform.position - who.transform.position;
        d.y = 0.0f;
        if (d.sqrMagnitude > 0.0001f) yield return GameAction_Face.Turn(who.transform, Quaternion.LookRotation(d, Vector3.up), 0.2f);
    }
}
