using System;
using System.Collections;
using UC;
using UC.Interaction;
using UnityEngine;

// Puts an item in the actor's inventory (the Inventory on the actor or its parents)
[Serializable]
[GameActionName("Inventory/Add Item")]
public class GameAction_AddToInventory : GameAction
{
    [SerializeField]
    private Item    item;
    [SerializeField, Min(1)]
    private int     quantity = 1;

    public override bool NeedWait() { return false; }

    public override IEnumerator Execute(ActionContext context)
    {
        var inventory = (context.triggerGameObject != null) ? (context.triggerGameObject.GetComponentInParent<Inventory>()) : (null);
        if (inventory == null)
        {
            Debug.LogWarning("Add Item: the actor has no Inventory");
            yield break;
        }
        if (item == null)
        {
            Debug.LogWarning("Add Item: no item set");
            yield break;
        }

        inventory.Add(item, quantity);
    }
}
