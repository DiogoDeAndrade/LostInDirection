using System;
using UC;
using UC.Interaction;
using UnityEngine;

// The actor carries an item (in the Inventory on the actor or its parents)
[Serializable]
[ConditionName("Inventory/Has Item")]
public class Condition_HasItem : Condition
{
    [SerializeField]
    private Item    item;
    [SerializeField, Min(1)]
    private int     count = 1;

    protected override bool EvaluateThis(ActionContext context)
    {
        if ((item == null) || (context.triggerGameObject == null)) return false;

        var inventory = context.triggerGameObject.GetComponentInParent<Inventory>();
        return (inventory != null) && (inventory.GetItemCount(item) >= count);
    }
}
