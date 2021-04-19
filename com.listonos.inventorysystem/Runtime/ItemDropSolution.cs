using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class ItemDropSolution<SlotEnum, ItemQualityEnum>
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public ItemDropSolution(InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem)
    {
      this.inventorySystem = inventorySystem;
    }

    public bool HasSolution
    {
      get
      {
        return currentSolutionSlots.Count > 0;
      }
    }

    private bool IsCurrentSolutionPerfectFit
    {
      get
      {
        return currentSolutionSlots.Count == 1 && inventorySystem.ItemFitsSlotPerfectly(item, currentSolutionSlots.First());
      }
    }

    public bool IsCurrentSolutionStackingFit
    {
      get
      {
        return currentSolutionSlots.Count > 0 && inventorySystem.ItemStacksInSlot(item, currentSolutionSlots.First());
      }
    }

    private HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>> currentSolutionSlots = new HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
    private HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>> validStackingSlots = new HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
    private HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>> validPerfectFitSlots = new HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
    private HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>> validPartialFitSlots = new HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
    private HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>> invalidSlots = new HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
    private List<ISlotCollection<SlotEnum, ItemQualityEnum>> slotCollections = new List<ISlotCollection<SlotEnum, ItemQualityEnum>>();
    private List<List<SlotBehaviour<SlotEnum, ItemQualityEnum>>> slotBlocksCandidates = new List<List<SlotBehaviour<SlotEnum, ItemQualityEnum>>>();

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;

    private ItemBehaviour<SlotEnum, ItemQualityEnum> item;

    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> GetSolutionSlots()
    {
      return currentSolutionSlots.ToList();
    }

    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> GetNotUsedSlots()
    {
      return validStackingSlots.Concat(validPerfectFitSlots).Concat(validPartialFitSlots).Concat(invalidSlots).ToList();
    }

    public void RegisterItem(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      this.item = item;
      Debug.Assert(currentSolutionSlots.Count == 0);
      Debug.Assert(validStackingSlots.Count == 0);
      Debug.Assert(validPerfectFitSlots.Count == 0);
      Debug.Assert(validPartialFitSlots.Count == 0);
      Debug.Assert(invalidSlots.Count == 0);
    }

    public void RegisterStartingSlots(List<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots)
    {
      if (slots.Count > 0)
      {
        foreach (var slot in slots)
        {
          if (slot.Collection != null && !slotCollections.Contains(slot.Collection))
          {
            slotCollections.Add(slot.Collection);
          }

          // Some of the partial fit slots can combine with other slots so keep them in partial fits as well
          if (!inventorySystem.ItemFitsSlotPerfectly(item, slot))
          {
            validPartialFitSlots.Add(slot);
          }

          currentSolutionSlots.Add(slot);
          slot.ShowDropHighlight(this);
        }
      }
    }

    public void AddSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      Debug.Assert(item != null);

      if (invalidSlots.Contains(slot) || validPerfectFitSlots.Contains(slot) || validStackingSlots.Contains(slot))
      {
        return;
      }

      if (slot.Collection != null && !slotCollections.Contains(slot.Collection))
      {
        slotCollections.Add(slot.Collection);
      }

      if (inventorySystem.ItemStacksInSlot(item, slot))
      {
        validStackingSlots.Add(slot);
        ConsiderValidSlotsForSolutionWithoutCacheUpdate();
      }
      else if (!inventorySystem.ItemCanGoIntoSlot(item, slot))
      {
        invalidSlots.Add(slot);
      }
      else if (inventorySystem.ItemFitsSlotPerfectly(item, slot))
      {
        validPerfectFitSlots.Add(slot);
        ConsiderValidSlotsForSolutionWithoutCacheUpdate();
      }
      else
      {
        if (!validPartialFitSlots.Contains(slot))
        {
          validPartialFitSlots.Add(slot);
          ConsiderValidSlotsForSolutionWithCacheUpdate();
        }
      }
    }

    public void RemoveSlot(SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      if (slot.Collection != null)
      {
        var collectionFoundInDifferentSlot = false;
        foreach (var otherSlot in validPerfectFitSlots.Concat(validPartialFitSlots).Concat(currentSolutionSlots))
        {
          if (!ReferenceEquals(otherSlot, slot) && ReferenceEquals(otherSlot.Collection, slot.Collection))
          {
            collectionFoundInDifferentSlot = true;
          }
        }

        if (!collectionFoundInDifferentSlot)
        {
          slotCollections.Remove(slot.Collection);
        }
      }

      if (currentSolutionSlots.Remove(slot))
      {
        slot.HideDropHighlight(this);
        InvalidateCurrentSolution();
        invalidSlots.Remove(slot);
        validPerfectFitSlots.Remove(slot);
        validPartialFitSlots.Remove(slot);
        ConsiderValidSlotsForSolutionWithCacheUpdate();
      }
      else
      {
        invalidSlots.Remove(slot);
        validPerfectFitSlots.Remove(slot);
        validPartialFitSlots.Remove(slot);
        validStackingSlots.Remove(slot);
      }
    }

    public void Clear()
    {
      item = null;
      foreach (var slot in currentSolutionSlots)
      {
        slot.HideDropHighlight(this);
      }
      currentSolutionSlots.Clear();
      validPerfectFitSlots.Clear();
      validPartialFitSlots.Clear();
      validStackingSlots.Clear();
      invalidSlots.Clear();
    }

    public void Update()
    {
      ConsiderValidSlotsForSolutionWithoutCacheUpdate();
    }

    private void InvalidateCurrentSolution()
    {
      if (HasSolution)
      {
        foreach (var slot in currentSolutionSlots)
        {
          slot.HideDropHighlight(this);
          if (inventorySystem.ItemStacksInSlot(item, slot))
          {
            validStackingSlots.Add(slot);
          }
          else if (inventorySystem.ItemFitsSlotPerfectly(item, slot))
          {
            validPerfectFitSlots.Add(slot);
          }
          else
          {
            if (!validPartialFitSlots.Contains(slot))
            {
              validPartialFitSlots.Add(slot);
            }
          }
        }
        currentSolutionSlots.Clear();
      }
    }

    private void UpdatePerfectFitSolution()
    {
      var closestValidPerfectFitSlot = GetClosestSlot(validPerfectFitSlots, item);
      if (IsCurrentSolutionPerfectFit)
      {
        if (CompareSlotsDistance(closestValidPerfectFitSlot, currentSolutionSlots.First()) < 0)
        {
          InvalidateCurrentSolution();
          currentSolutionSlots.Add(closestValidPerfectFitSlot);

          foreach (var slot in currentSolutionSlots)
          {
            slot.ShowDropHighlight(this);
          }
        }
      }
      else
      {
        InvalidateCurrentSolution();
        currentSolutionSlots.Add(closestValidPerfectFitSlot);

        foreach (var slot in currentSolutionSlots)
        {
          slot.ShowDropHighlight(this);
        }
      }
    }

    private void UpdateStackingFitSolution()
    {
      var closestValidStackingSlot = GetClosestSlot(validStackingSlots, item);
      if (IsCurrentSolutionStackingFit)
      {
        var closestCurrentSolutionSlot = GetClosestSlot(currentSolutionSlots, item);
        if (CompareSlotsDistance(closestValidStackingSlot, closestCurrentSolutionSlot) < 0)
        {
          InvalidateCurrentSolution();
          var currentItem = inventorySystem.GetItemInSlot(closestValidStackingSlot);
          var slots = inventorySystem.GetSlotsHavingItem(currentItem);

          foreach (var slot in slots)
          {
            currentSolutionSlots.Add(slot);
            slot.ShowDropHighlight(this);
          }
        }
      }
      else
      {
        InvalidateCurrentSolution();
        var currentItem = inventorySystem.GetItemInSlot(closestValidStackingSlot);
        var slots = inventorySystem.GetSlotsHavingItem(currentItem);

        foreach (var slot in slots)
        {
          currentSolutionSlots.Add(slot);
          slot.ShowDropHighlight(this);
        }
      }
    }

    private void CachePartialFitSolutions()
    {
      slotBlocksCandidates.Clear();

      foreach (var slotCollection in slotCollections)
      {
        var validSlotsInCollection = slotCollection.FilterSlotsPartOfCollection(validPartialFitSlots);
        if (validSlotsInCollection.Count > 0)
        {
          slotBlocksCandidates.AddRange(slotCollection.FindSlotsBlocksFittingDimensionsFromList(validSlotsInCollection, item.ItemDatum.Size));
        }
      }
    }

    private void UpdatePartialFitSolution()
    {
      var slotBlocksCandidatesCenter = new List<Vector2>(slotBlocksCandidates.Count);
      foreach (var slotBlockCandidates in slotBlocksCandidates)
      {
        Debug.Assert(slotBlockCandidates.Count > 0);
        slotBlocksCandidatesCenter.Add(FindCenter(slotBlockCandidates));
      }

      Debug.Assert(slotBlocksCandidatesCenter.Count == slotBlocksCandidates.Count);
      var closestSlotBlock = 0;
      for (int i = 1; i < slotBlocksCandidates.Count; i++)
      {
        if (ComparePositionsDistanceToItem(slotBlocksCandidatesCenter[i], slotBlocksCandidatesCenter[closestSlotBlock]) < 0)
        {
          closestSlotBlock = i;
        }
      }

      var candidateSlotBlock = slotBlocksCandidates[closestSlotBlock];
      var candidateSlotBlockCenter = slotBlocksCandidatesCenter[closestSlotBlock];

      if (HasSolution)
      {
        var currentSolutionCenter = FindCenter(currentSolutionSlots);
        if (ComparePositionsDistanceToItem(candidateSlotBlockCenter, currentSolutionCenter) < 0)
        {
          InvalidateCurrentSolution();
          foreach (var slot in candidateSlotBlock)
          {
            currentSolutionSlots.Add(slot);
            slot.ShowDropHighlight(this);
          }
        }
      }
      else
      {
        foreach (var slot in candidateSlotBlock)
        {
          currentSolutionSlots.Add(slot);
          slot.ShowDropHighlight(this);
        }
      }
    }

    private void ConsiderValidSlotsForSolutionWithoutCacheUpdate()
    {
      // Prefer perfect fits
      if (validPerfectFitSlots.Count > 0)
      {
        UpdatePerfectFitSolution();
      }
      else if (validStackingSlots.Count > 0)
      {
        UpdateStackingFitSolution();
      }
      else if (slotBlocksCandidates.Count > 0)
      {
        UpdatePartialFitSolution();
      }
    }

    private void ConsiderValidSlotsForSolutionWithCacheUpdate()
    {
      CachePartialFitSolutions();

      // Prefer perfect fits
      if (validPerfectFitSlots.Count > 0)
      {
        UpdatePerfectFitSolution();
      }
      else if (slotBlocksCandidates.Count > 0)
      {
        UpdatePartialFitSolution();
      }
    }

    private Vector2 GetSlotPosition(SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      return slot.GetPosition();
    }

    private Vector2 GetItemPosition(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      return item.GetPosition();
    }

    private int ComparePositionsDistanceToItem(Vector2 positionA, Vector2 positionB)
    {
      var itemPosition = GetItemPosition(item);
      var aDistance = Vector2.Distance(positionA, itemPosition);
      var bDistance = Vector2.Distance(positionB, itemPosition);
      return aDistance.CompareTo(bDistance);
    }

    private int CompareSlotsDistance(SlotBehaviour<SlotEnum, ItemQualityEnum> slotA, SlotBehaviour<SlotEnum, ItemQualityEnum> slotB)
    {
      return ComparePositionsDistanceToItem(GetSlotPosition(slotA), GetSlotPosition(slotB));
    }

    private SlotBehaviour<SlotEnum, ItemQualityEnum> GetClosestSlot(HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots, ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      Debug.Assert(slots.Count > 0);
      return slots.Aggregate(slots.First(), (closestSlot, currentSlot) =>
      {
        if (CompareSlotsDistance(currentSlot, closestSlot) < 0)
        {
          return currentSlot;
        }
        else
        {
          return closestSlot;
        }
      });
    }

    private Vector2 FindCenter(IEnumerable<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots)
    {
      Debug.Assert(slots.Count() > 0);
      var positionSum = slots.Aggregate(Vector2.zero, (positionSum, slot) =>
      {
        return positionSum + new Vector2(slot.transform.position.x, slot.transform.position.y);
      });
      return positionSum / slots.Count();
    }
  }
}
