using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Spawns a prefab where the first object with a hypertag is (a spawn point placed in the scene)
[Serializable]
[GameActionName("Object/Instantiate at Hypertag")]
public class GameAction_InstantiateAtHypertag : GameAction
{
    [SerializeField]
    private GameObject  prefab;
    [SerializeField, Tooltip("Spawned at the first object with this hypertag")]
    private Hypertag    spawnPoint;
    [SerializeField, Tooltip("Take the spawn point's rotation (otherwise the prefab's own)")]
    private bool        useRotation = true;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Instantiate: no prefab set");
            yield break;
        }

        var point = (spawnPoint != null) ? (Hypertag.FindFirstObjectWithHypertag<Transform>(spawnPoint)) : (null);
        if (point == null)
        {
            Debug.LogWarning($"Instantiate: no object with hypertag {((spawnPoint != null) ? (spawnPoint.name) : ("(none)"))} to spawn {prefab.name} at");
            yield break;
        }

        Quaternion rotation = (useRotation) ? (point.rotation) : (prefab.transform.rotation);
        GameObject.Instantiate(prefab, point.position, rotation);
    }
}
