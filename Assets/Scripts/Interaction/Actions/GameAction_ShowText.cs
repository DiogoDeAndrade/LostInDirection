using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Floats a text over an object (a gravestone's epitaph, ...), through UnityCommon's CombatTextManager: it stays,
// then fades out while drifting up
[Serializable]
[GameActionName("UI/Show Text")]
public class GameAction_ShowText : GameAction
{
    [SerializeField, TextArea(2, 5)]
    private string          text;
    [SerializeField]
    private ObjectTarget    target = new ObjectTarget(ObjectTarget.Mode.Interactable);
    [SerializeField, Tooltip("World offset from the object, up being y")]
    private Vector2         offset = new Vector2(0.0f, 0.6f);
    [SerializeField]
    private Color           color = Color.white;
    [SerializeField, Min(0.1f), Tooltip("Seconds it's up, the last half fading out")]
    private float           duration = 4.0f;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var obj = target.GetGameObject(context);
        if (obj == null)
        {
            Debug.LogWarning($"Show Text: no object ({target})");
            yield break;
        }

        var def = new CombatTextDef(color, duration) { followOwner = false };
        CombatTextManager.SpawnText(obj, offset, text, def);
    }
}
