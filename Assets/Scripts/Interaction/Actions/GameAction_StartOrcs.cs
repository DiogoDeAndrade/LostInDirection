using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Starts an OrcSpawner sending its orcs in
[Serializable]
[GameActionName("Orcs/Start Spawning")]
public class GameAction_StartOrcs : GameAction
{
    [SerializeField]
    private OrcSpawner  spawner;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        if (spawner == null)
        {
            Debug.LogWarning("Start Spawning: no spawner");
            yield break;
        }

        spawner.Begin();
    }
}
