using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class ItemWithStackCounter<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject StackCountersRoot;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public Sprite[] DigitSprites = new Sprite[10];
    public bool ShowCounterWhenOne;

    public string DraggingSortingLayerName = "Default";

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private int defaultSortingLayerId;
    private int draggingSortingLayerId;

    private List<SpriteRenderer> digitSpriteRenderers = new List<SpriteRenderer>();
    private bool countersEnabled = false;

    void Awake()
    {
      Debug.AssertFormat(StackCountersRoot != null, "ItemWithStackCounter behavior expects valid reference to StackCountersRoot game object.");
      digitSpriteRenderers = StackCountersRoot.GetComponentsInChildren<SpriteRenderer>().ToList();
      Debug.AssertFormat(digitSpriteRenderers.Count > 0, "ItemWithStackCounter behavior expects StackCountersRoot game object to have at least one child with SpriteRenderer behavior.");
      defaultSortingLayerId = digitSpriteRenderers[0].sortingLayerID;
      draggingSortingLayerId = SortingLayer.NameToID(DraggingSortingLayerName);
      Debug.AssertFormat(draggingSortingLayerId != 0, "ItemWithStackCounter behavior expects DraggingSortingLayerName to be valid sorting layer name.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemWithStackCounter behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemWithStackCounter behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      inventorySystem.ItemStacksChanged += InventorySystem_ItemStacksChanged;
      inventorySystem.ItemBeingDestroyed += InventorySystem_ItemBeingDestroyed;
    }

    private void InventorySystem_ItemStacksChanged(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemStacksChangedEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        UpdateCounters();
      }
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      countersEnabled = ItemBehaviour.ItemDatum.Stacks;
      if (countersEnabled)
      {
        Debug.AssertFormat(GetDigitsBackToFront(ItemBehaviour.ItemDatum.StackLimit).Count() <= digitSpriteRenderers.Count, "ItemWithStackCounter behavior expected to find enough children of StackCountersRoot with SpriteRenderer to account for Item's StackLimit.");
      }
      UpdateCounters();
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (countersEnabled && ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        foreach (var spriteRenderer in digitSpriteRenderers)
        {
          spriteRenderer.sortingLayerID = draggingSortingLayerId;
        }
      }

    }
    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (countersEnabled && ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        foreach (var spriteRenderer in digitSpriteRenderers)
        {
          spriteRenderer.sortingLayerID = defaultSortingLayerId;
        }
      }
    }

    private void UpdateCounters()
    {
      if (countersEnabled)
      {
        StackCountersRoot.SetActive(true);
        Debug.AssertFormat(ItemBehaviour.Stacks >= 1, "ItemWithStackCounter behavior expected ItemBehaviour.Stacks >= 1.");
        if (ItemBehaviour.Stacks == 1 && !ShowCounterWhenOne)
        {
          foreach (var spriteRenderer in digitSpriteRenderers)
          {
            spriteRenderer.sprite = null;
          }
        }
        else
        {
          var digitsBackToFront = GetDigitsBackToFront(ItemBehaviour.Stacks).ToList();
          if (digitsBackToFront.Count > digitSpriteRenderers.Count)
          {
            foreach (var spriteRenderer in digitSpriteRenderers)
            {
              spriteRenderer.sprite = DigitSprites[9];
            }
          }
          else
          {
            for (int i = 0; i < digitsBackToFront.Count; i++)
            {
              digitSpriteRenderers[i].sprite = DigitSprites[digitsBackToFront[i]];
            }

            for (int i = digitsBackToFront.Count; i < digitSpriteRenderers.Count; i++)
            {
              digitSpriteRenderers[i].sprite = null;
            }
          }
        }
      }
      else
      {
        StackCountersRoot.SetActive(false);
      }
    }

    // Source: https://stackoverflow.com/questions/45508659/get-separate-digits-from-int-in-c-sharp
    private static IEnumerable<int> GetDigitsBackToFront(int source)
    {
      while (source > 0)
      {
        var digit = source % 10;
        source /= 10;
        yield return digit;
      }
    }

    private void InventorySystem_ItemBeingDestroyed(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemBeingDestroyedEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, this))
      {
        inventorySystem.AfterDataReady -= InventorySystem_AfterDataReady;
        inventorySystem.ItemStartedDragging -= InventorySystem_ItemStartedDragging;
        inventorySystem.ItemStoppedDragging -= InventorySystem_ItemStoppedDragging;
        inventorySystem.ItemBeingDestroyed -= InventorySystem_ItemBeingDestroyed;
      }
    }
  }
}