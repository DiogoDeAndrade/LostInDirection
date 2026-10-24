using System;
using UC;
using UC.Interaction;
using UnityEngine;

// Which object an action works on: the interactable, the actor, a specific object, or the first one with a hypertag
[Serializable]
public class ObjectTarget
{
    public enum Mode { Interactable = 0, Actor = 1, Object = 2, Hypertag = 3 };

    [SerializeField]
    private Mode        mode = Mode.Interactable;
    [SerializeField, Tooltip("Used when the mode is Object")]
    private GameObject  obj;
    [SerializeField, Tooltip("Used when the mode is Hypertag")]
    private Hypertag    hypertag;

    public ObjectTarget() { }
    public ObjectTarget(Mode mode) { this.mode = mode; }

    public GameObject GetGameObject(ActionContext context)
    {
        switch (mode)
        {
            case Mode.Interactable:
                return context.targetGameObject;
            case Mode.Actor:
                return context.triggerGameObject;
            case Mode.Object:
                return obj;
            case Mode.Hypertag:
                {
                    var tagged = (hypertag != null) ? (Hypertag.FindFirstObjectWithHypertag<Transform>(hypertag)) : (null);
                    return (tagged != null) ? (tagged.gameObject) : (null);
                }
            default:
                return null;
        }
    }

    public override string ToString() => mode.ToString();
}
