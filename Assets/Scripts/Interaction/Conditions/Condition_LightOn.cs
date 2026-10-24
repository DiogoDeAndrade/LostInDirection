using System;
using UC;
using UC.Interaction;
using UnityEngine;

// A light on the object (or its children) is on (negate it for "is off")
[Serializable]
[ConditionName("Object/Light Is On")]
public class Condition_LightOn : Condition
{
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Interactable);

    protected override bool EvaluateThis(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        if (obj == null) return false;

        foreach (var light in obj.GetComponentsInChildren<Light>())
        {
            if (light.enabled) return true;
        }
        return false;
    }
}
