using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Switches the lights on an object (every Light on it and its children) on, off, or the other way round
[Serializable]
[GameActionName("Object/Set Light")]
public class GameAction_SetLight : GameAction
{
    public enum State { On = 0, Off = 1, Toggle = 2 };

    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Interactable);
    [SerializeField]
    private State           state = State.Off;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        var lights = (obj != null) ? (obj.GetComponentsInChildren<Light>(true)) : (null);
        if ((lights == null) || (lights.Length == 0))
        {
            Debug.LogWarning($"Set Light: no light ({target})");
            yield break;
        }

        foreach (var light in lights)
        {
            light.enabled = (state == State.Toggle) ? (!light.enabled) : (state == State.On);
        }
    }
}
