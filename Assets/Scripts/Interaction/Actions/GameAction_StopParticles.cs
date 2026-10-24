using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Stops every particle system on an object and its children: they stop emitting, and the particles already out
// either finish their lives or (clear) disappear at once
[Serializable]
[GameActionName("FX/Stop Particle System")]
public class GameAction_StopParticles : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Interactable);
    [SerializeField, Tooltip("Remove the particles already out, instead of letting them finish")]
    private bool            clear = false;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        var systems = (obj != null) ? (obj.GetComponentsInChildren<ParticleSystem>()) : (null);
        if ((systems == null) || (systems.Length == 0))
        {
            Debug.LogWarning($"Stop Particle System: no particle system ({target})");
            yield break;
        }

        var behavior = (clear) ? (ParticleSystemStopBehavior.StopEmittingAndClear) : (ParticleSystemStopBehavior.StopEmitting);
        foreach (var system in systems) system.Stop(true, behavior);
    }
}
