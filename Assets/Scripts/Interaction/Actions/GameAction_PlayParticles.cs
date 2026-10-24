using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Plays every particle system on an object and its children (a candle's three flames, the dig dirt, ...). With
// "wait", the next action starts when all their particles are gone.
[Serializable]
[GameActionName("FX/Play Particle System")]
public class GameAction_PlayParticles : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Object);

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        var systems = (obj != null) ? (obj.GetComponentsInChildren<ParticleSystem>()) : (null);
        if ((systems == null) || (systems.Length == 0))
        {
            Debug.LogWarning($"Play Particle System: no particle system ({target})");
            yield break;
        }

        foreach (var system in systems) system.Play(true);

        bool alive = true;
        while (alive)
        {
            alive = false;
            foreach (var system in systems)
            {
                if ((system != null) && (system.IsAlive(true))) alive = true;
            }
            if (alive) yield return null;
        }
    }
}
