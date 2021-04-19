using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.InventorySystem
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
    public abstract void LoadData();

    public ItemBehaviour<SlotEnum, ItemQualityEnum> DraggedItem { get; private set; }
    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> DraggingSources { get; private set; } = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>();

    public class ItemDragEventArgs : EventArgs
    {
      public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> SourceSlotBehaviors { get; set; }
      public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour { get; set; }
      public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> TargetDropSlotBehavior { get; set; }
    }

    public class ItemStacksChangedEventArgs : EventArgs
    {
      public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour { get; set; }
      public int NewStackCount { get; set; }
    }

    public class ItemBeingDestroyedEventArgs : EventArgs
    {
      public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour { get; set; }
    }

    public event EventHandler<ItemDragEventArgs> ItemStartedDragging;
    public event EventHandler<ItemDragEventArgs> ItemStoppedDragging;
    public event EventHandler<ItemStacksChangedEventArgs> ItemStacksChanged;
    public event EventHandler<ItemBeingDestroyedEventArgs> ItemBeingDestroyed;
    public event EventHandler DataReady;
    public event EventHandler AfterDataReady;

    private Dictionary<SlotBehaviour<SlotEnum, ItemQualityEnum>, ItemBehaviour<SlotEnum, ItemQualityEnum>> slotToItemDictionary = new Dictionary<SlotBehaviour<SlotEnum, ItemQualityEnum>, ItemBehaviour<SlotEnum, ItemQualityEnum>>();

    private Dictionary<ItemBehaviour<SlotEnum, ItemQualityEnum>, List<SlotBehaviour<SlotEnum, ItemQualityEnum>>> itemToSlotsDictionary = new Dictionary<ItemBehaviour<SlotEnum, ItemQualityEnum>, List<SlotBehaviour<SlotEnum, ItemQualityEnum>>>();

    private ItemDropSolution<SlotEnum, ItemQualityEnum> itemDropSolution;

    public virtual void Awake()
    {
      LoadData();
    }

    public virtual void Start()
    {
      OnDataReady(new EventArgs());
      itemDropSolution = new ItemDropSolution<SlotEnum, ItemQualityEnum>(this);
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
      if (DraggingItem)
      {
        itemDropSolution.Update();
      }
    }

    public virtual void OnDataReady(EventArgs args)
    {
      DataReady?.Invoke(this, args);
      AfterDataReady?.Invoke(this, args);
    }

    public ItemBehaviour<SlotEnum, ItemQualityEnum> GetItemInSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour)
    {
      ItemBehaviour<SlotEnum, ItemQualityEnum> item;
      slotToItemDictionary.TryGetValue(slotBehaviour, out item);
      return item;
    }

    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> GetSlotsHavingItem(ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      List<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots;
      itemToSlotsDictionary.TryGetValue(itemBehaviour, out slots);
      return slots;
    }

    public bool ItemStacksInSlot(ItemBehaviour<SlotEnum, ItemQualityEnum> item, SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      var currentItemInSlot = GetItemInSlot(slot);
      return currentItemInSlot != null && !ReferenceEquals(currentItemInSlot, item) && currentItemInSlot.Item == item.Item && currentItemInSlot.ItemDatum.Stacks && item.Stacks + currentItemInSlot.Stacks <= currentItemInSlot.ItemDatum.StackLimit;
    }

    public bool ItemCanGoIntoSlot(ItemBehaviour<SlotEnum, ItemQualityEnum> item, SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      var currentItemInSlot = GetItemInSlot(slot);
      var occupiedCheck = currentItemInSlot == null || ReferenceEquals(currentItemInSlot, slot);
      var slotMatchCheck = (item.ItemDatum.HasItemSlot && Equals(item.ItemDatum.ItemSlot, slot.Slot));
      var slotCheck = slot.SlotDatum.AllowAllItems || slotMatchCheck;
      return occupiedCheck && slotCheck;
    }

    public bool ItemFitsSlotPerfectly(ItemBehaviour<SlotEnum, ItemQualityEnum> item, SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      return item.ItemDatum.Size.x <= slot.SlotDatum.Size.x && item.ItemDatum.Size.y <= slot.SlotDatum.Size.y;
    }

    public void StartDraggingItem(ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      DraggedItem = itemBehaviour;
      itemDropSolution.RegisterItem(DraggedItem);
      if (itemToSlotsDictionary.ContainsKey(itemBehaviour))
      {
        DraggingSources = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>(itemToSlotsDictionary[itemBehaviour]);
        itemDropSolution.RegisterStartingSlots(DraggingSources);
      }
      ItemStartedDragging?.Invoke(this, new ItemDragEventArgs() { ItemBehaviour = itemBehaviour, SourceSlotBehaviors = DraggingSources, TargetDropSlotBehavior = null}); ;
    }

    public void StopDraggingItem(ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      Debug.Assert(itemBehaviour == DraggedItem);
      var eventArgs = new ItemDragEventArgs() { ItemBehaviour = itemBehaviour, SourceSlotBehaviors = DraggingSources };

      if (itemDropSolution.HasSolution)
      {
        foreach (var draggingSource in DraggingSources)
        {
          slotToItemDictionary.Remove(draggingSource);
        }

        if (itemDropSolution.IsCurrentSolutionStackingFit)
        {
          var solutionSlots = itemDropSolution.GetSolutionSlots();

          foreach (var slot in itemDropSolution.GetNotUsedSlots())
          {
            ItemStoppedOverlapWithSlot(slot, itemBehaviour);
          }

          eventArgs.TargetDropSlotBehavior = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>(solutionSlots);
          ItemStoppedDragging?.Invoke(this, eventArgs);

          var targetItem = slotToItemDictionary[solutionSlots[0]];
          targetItem.IncreaseStacks(itemBehaviour.Stacks);
          ItemStacksChanged?.Invoke(this, new ItemStacksChangedEventArgs() { ItemBehaviour = targetItem, NewStackCount = targetItem.Stacks });

          DestroyItem(itemBehaviour);
        }
        else
        {
          var solutionSlots = itemDropSolution.GetSolutionSlots();
          itemBehaviour.SetPositionToSlots(solutionSlots);

          foreach (var solutionSlot in solutionSlots)
          {
            slotToItemDictionary[solutionSlot] = itemBehaviour;
          }

          itemToSlotsDictionary[itemBehaviour] = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>(solutionSlots);

          foreach (var slot in itemDropSolution.GetNotUsedSlots())
          {
            ItemStoppedOverlapWithSlot(slot, itemBehaviour);
          }

          eventArgs.TargetDropSlotBehavior = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>(solutionSlots);
          ItemStoppedDragging?.Invoke(this, eventArgs);
        }
      }
      else
      {
        itemBehaviour.ResetPositionToStartDrag();
        foreach (var slot in itemDropSolution.GetNotUsedSlots())
        {
          ItemStoppedOverlapWithSlot(slot, itemBehaviour);
        }
        ItemStoppedDragging?.Invoke(this, eventArgs);
      }

      DraggedItem = null;
      DraggingSources.Clear();
      itemDropSolution.Clear();
    }

    public void ItemBeginOverlapWithSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      if (itemBehaviour == DraggedItem)
      {
        itemDropSolution.AddSlot(slotBehaviour);
      }
    }

    public void ItemStoppedOverlapWithSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slotBehaviour, ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      if (itemBehaviour == DraggedItem)
      {
        itemDropSolution.RemoveSlot(slotBehaviour);
      }
    }

    public void DestroyItem(ItemBehaviour<SlotEnum, ItemQualityEnum> itemBehaviour)
    {
      List<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots;
      if (itemToSlotsDictionary.TryGetValue(itemBehaviour, out slots))
      {
        foreach (var slot in slots)
        {
          slotToItemDictionary.Remove(slot);
        }
        itemToSlotsDictionary.Remove(itemBehaviour);
      }

      ItemBeingDestroyed?.Invoke(this, new ItemBeingDestroyedEventArgs() { ItemBehaviour = itemBehaviour });

      Destroy(itemBehaviour.gameObject);
    }
  }
}