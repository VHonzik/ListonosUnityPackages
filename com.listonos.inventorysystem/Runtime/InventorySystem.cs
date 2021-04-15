using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public abstract class InventorySystem<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    [Serializable]
    public struct StartingItemPlacement
    {
      public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
      public SlotBehaviour<SlotEnum, ItemQualityEnum>[] Slots;
    }
    public StartingItemPlacement[] StartingItemsPlacement;

    public bool DraggingItem
    {
      get
      {
        return DraggedItem != null;
      }
    }

    public abstract SlotDatum<SlotEnum> GetSlotDatum(SlotEnum slot);
    public abstract ItemQualityDatum<ItemQualityEnum> GetItemQualityDatum(ItemQualityEnum itemQuality);
    public abstract ItemDatum<SlotEnum, ItemQualityEnum> GetItemDatum(string item);

    public ItemBehaviour<SlotEnum, ItemQualityEnum> DraggedItem { get; private set; }
    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> DraggingSources { get; private set; } = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>();

    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> DropTargets { get; private set; } = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>();

    public class ItemDragEventArgs : EventArgs
    {
      public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> SourceSlotBehaviors { get; set; }
      public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour { get; set; }
      public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> TargetDropSlotBehavior { get; set; }
    }

    public event EventHandler<ItemDragEventArgs> ItemStartedDragging;
    public event EventHandler<ItemDragEventArgs> ItemStoppedDragging;

    private Dictionary<SlotBehaviour<SlotEnum, ItemQualityEnum>, ItemBehaviour<SlotEnum, ItemQualityEnum>> slotToItemDictionary = new Dictionary<SlotBehaviour<SlotEnum, ItemQualityEnum>, ItemBehaviour<SlotEnum, ItemQualityEnum>>();

    private Dictionary<ItemBehaviour<SlotEnum, ItemQualityEnum>, List<SlotBehaviour<SlotEnum, ItemQualityEnum>>> itemToSlotsDictionary = new Dictionary<ItemBehaviour<SlotEnum, ItemQualityEnum>, List<SlotBehaviour<SlotEnum, ItemQualityEnum>>>();

    void Start()
    {
      foreach (var startingItem in StartingItemsPlacement)
      {
        foreach (var slotBehaviour in startingItem.Slots)
        {
          slotToItemDictionary[slotBehaviour] = startingItem.ItemBehaviour;
        }
        itemToSlotsDictionary[startingItem.ItemBehaviour] = startingItem.Slots.ToList();
      }
    }

    void Update()
    {
      if (DraggingItem && DropTargets.Count > 0)
      {
        SortDropTargetsAndUpdateSlots();
      }
    }

    public void StartDraggingItem(ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      DraggedItem = itemBehaviour;
      if (itemToSlotsDictionary.ContainsKey(itemBehaviour))
      {
        DraggingSources = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>(itemToSlotsDictionary[itemBehaviour]);
      }
      ItemStartedDragging?.Invoke(this, new ItemDragEventArgs() { ItemBehaviour = itemBehaviour, SourceSlotBehaviors = DraggingSources, TargetDropSlotBehavior = null });
    }

    public void StopDraggingItem(ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      Debug.Assert(itemBehaviour == DraggedItem);
      var eventArgs = new ItemDragEventArgs() { ItemBehaviour = itemBehaviour, SourceSlotBehaviors = DraggingSources, TargetDropSlotBehavior = DropTargets };

      var requiredSlots = itemBehaviour.ItemDatum.Size.x * itemBehaviour.ItemDatum.Size.y;
      if (DropTargets.Count > 0 && DropTargets.Count >= requiredSlots)
      {
        foreach (var draggingSource in DraggingSources)
        {
          draggingSource.RemoveItem();
          slotToItemDictionary.Remove(draggingSource);
        }

        var neededDropTargers = DropTargets.GetRange(0, requiredSlots);

        foreach (var dropTarget in neededDropTargers)
        {
          dropTarget.DropItem(itemBehaviour);
          slotToItemDictionary[dropTarget] = itemBehaviour;
        }

        itemToSlotsDictionary[itemBehaviour] = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>(neededDropTargers);

        for (int i = DropTargets.Count-1; i >= requiredSlots; i--)
        {
          ItemStoppedOverlapWithSlot(DropTargets[i], itemBehaviour);
        }

        itemBehaviour.SetPositionToSlots(neededDropTargers);
      }
      else
      {
        itemBehaviour.ResetPositionToStartDrag();
        for (int i = DropTargets.Count - 1; i >= 0; i--)
        {
          ItemStoppedOverlapWithSlot(DropTargets[i], itemBehaviour);
        }
      }

      ItemStoppedDragging?.Invoke(this, eventArgs);

      DraggedItem = null;
      DraggingSources.Clear();
      DropTargets.Clear();
    }

    public void ItemBeginOverlapWithSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      if (itemBehaviour == DraggedItem && (DraggingSources.Contains(slotBehaviour) || (slotBehaviour.AcceptsItem(itemBehaviour) && !slotToItemDictionary.ContainsKey(slotBehaviour))))
      {
        Debug.Assert(!DropTargets.Contains(slotBehaviour));
        DropTargets.Add(slotBehaviour);
        SortDropTargetsAndUpdateSlots();
      }
    }

    public void ItemStoppedOverlapWithSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {

      if (itemBehaviour == DraggedItem && (DraggingSources.Contains(slotBehaviour) || (slotBehaviour.AcceptsItem(itemBehaviour) && !slotToItemDictionary.ContainsKey(slotBehaviour))))
      {
        Debug.Assert(DropTargets.Contains(slotBehaviour));
        DropTargets.Remove(slotBehaviour);
        slotBehaviour.ItemExitedDrop();
        SortDropTargetsAndUpdateSlots();
      }
    }
    private void SortDropTargetsAndUpdateSlots()
    {
      DropTargets.Sort((slotA, slotB) =>
      {
        var aDistance = Vector2.Distance(slotA.transform.position, DraggedItem.transform.position);
        var bDistance = Vector2.Distance(slotB.transform.position, DraggedItem.transform.position);
        return aDistance.CompareTo(bDistance);
      });

      var requiredSlots = DraggedItem.ItemDatum.Size.x * DraggedItem.ItemDatum.Size.y;
      var validSlotCount = Math.Min(requiredSlots, DropTargets.Count);
      for (int i = 0; i < validSlotCount; i++)
      {
        DropTargets[i].ItemEnteredDrop();
      }

      for (int i = validSlotCount; i < DropTargets.Count; i++)
      {
        DropTargets[i].ItemExitedDrop();
      }
    }
  }
}