using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public abstract class InventorySystem<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public abstract SlotDatum<SlotEnum> GetSlotDatum(SlotEnum slot);
    public abstract ItemQualityDatum<ItemQualityEnum> GetItemQualityDatum(ItemQualityEnum itemQuality);
    public abstract ItemDatum<ItemQualityEnum> GetItemDatum(string item);

    public bool DraggingItem
    {
      get
      {
        return DraggedItem != null;
      }
    }
    public ItemBehaviour<SlotEnum, ItemQualityEnum> DraggedItem { get; private set; }
    public SlotBehaviour<SlotEnum, ItemQualityEnum> DraggingSource { get; private set; }

    public SlotBehaviour<SlotEnum, ItemQualityEnum> DropTarget { get; private set; }

    public class ItemDragEventArgs : EventArgs
    {
      public SlotBehaviour<SlotEnum, ItemQualityEnum> sourceSlotBehavior { get; set; }
      public ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour { get; set; }
    }

    public event EventHandler<ItemDragEventArgs> ItemStartedDragging;

    public void StartDraggingItem(SlotBehaviour<SlotEnum, ItemQualityEnum> sourceSlotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      itemBehaviour.StartDragging(Input.mousePosition);
      DraggedItem = itemBehaviour;
      DraggingSource = sourceSlotBehaviour;
      ItemStartedDragging?.Invoke(this, new ItemDragEventArgs() { itemBehaviour = itemBehaviour, sourceSlotBehavior = sourceSlotBehaviour });
    }

    public void ContinueDraggingItem(SlotBehaviour<SlotEnum, ItemQualityEnum> sourceSlotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      Debug.Assert(sourceSlotBehaviour == DraggingSource);
      itemBehaviour.Dragging(Input.mousePosition);
    }

    public void StopDraggingItem(SlotBehaviour<SlotEnum, ItemQualityEnum> sourceSlotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      Debug.Assert(sourceSlotBehaviour == DraggingSource);

      if (DropTarget != null)
      {
        DraggingSource.RemoveItem(itemBehaviour);
        DropTarget.DropItem(itemBehaviour);
      }

      DraggedItem = null;
      DraggingSource = null;
      DropTarget = null;
      itemBehaviour.StopDragging();
    }

    public void ItemBeginOverlapWithSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      if (itemBehaviour == DraggedItem && slotBehaviour != DraggingSource)
      {
        DropTarget = slotBehaviour;
        slotBehaviour.ItemEnteredDrop(itemBehaviour);
      }
    }

    public void ItemStoppedOverlapWithSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      if (itemBehaviour == DraggedItem && slotBehaviour != DraggingSource)
      {
        DropTarget = null;
        slotBehaviour.ItemExitedDrop(itemBehaviour);
      }
    }
  }
}