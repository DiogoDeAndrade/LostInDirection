using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Plays a particle system (the first one on the object or its children). With "wait", the next action starts when
// all its particles are gone.
[Serializable]
[GameActionName("FX/Play Particle System")]
public class GameAction_PlayParticles : GameAction
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Object);

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        var particles = (obj != null) ? (obj.GetComponentInChildren<ParticleSystem>()) : (null);
        if (particles == null)
        {
            Debug.LogWarning($"Play Particle System: no particle system ({target})");
            yield break;
        }

        particles.Play(true);

        while ((particles != null) && (particles.IsAlive(true))) yield return null;
    }
}
