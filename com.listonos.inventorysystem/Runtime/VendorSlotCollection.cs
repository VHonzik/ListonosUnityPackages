using System;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class VendorSlotCollection<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public InventorySystem<SlotEnum, ItemQualityEnum> InventorySystem;
    public GridSlotCollection<SlotEnum, ItemQualityEnum> SlotCollection;
    public bool DestroyItemOnDrop;

    public class ItemEventArgs : EventArgs
    {
      public ItemBehaviour<SlotEnum, ItemQualityEnum> Item;
    }

    public event EventHandler<ItemEventArgs> ItemSold;
    public event EventHandler<ItemEventArgs> ItemBought;

    void Awake()
    {
      Debug.AssertFormat(InventorySystem != null, "VendorSlotCollection behavior expects valid InventorySystem reference.");
      InventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
    }

    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (e.TargetDropSlotBehavior.Count > 0)
      {
        var firstSlot = e.TargetDropSlotBehavior[0];
        var item = e.ItemBehaviour;
        if (SlotCollection.Contains(firstSlot))
        {
          foreach (var dropSlot in e.TargetDropSlotBehavior)
          {
            Debug.AssertFormat(SlotCollection.Contains(dropSlot), "VendorSlotCollection received an item drop to slots where some were part of the collection and some weren't.");
          }

          ItemSold?.Invoke(this, new ItemEventArgs() { Item = item });

          if (DestroyItemOnDrop)
          {
            InventorySystem.DestroyItem(item);
          }
        }
      }

      if (e.SourceSlotBehaviors.Count > 0)
      {
        var firstSlot = e.SourceSlotBehaviors[0];
        var item = e.ItemBehaviour;

        if (SlotCollection.Contains(firstSlot))
        {
          foreach (var dropSlot in e.SourceSlotBehaviors)
          {
            Debug.AssertFormat(SlotCollection.Contains(dropSlot), "VendorSlotCollection received an item drop to slots where some were part of the collection and some weren't.");
          }

          ItemBought?.Invoke(this, new ItemEventArgs() { Item = item });
        }
      }
    }
  }
}
