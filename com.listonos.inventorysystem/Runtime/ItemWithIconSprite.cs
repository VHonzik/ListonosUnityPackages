using System;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class ItemWithIconSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject IconSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public string DraggingSortingLayerName = "Default";

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer iconSpriteRenderer;
    private int defaultSortingLayerId;
    private int draggingSortingLayerId;

    void Awake()
    {
      Debug.AssertFormat(IconSprite != null, "ItemWithIconSprite behavior expects valid reference to IconSprite game object.");
      iconSpriteRenderer = IconSprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(iconSpriteRenderer != null, "ItemWithIconSprite behavior expects IconSprite game object to have SpriteRenderer behavior.");
      defaultSortingLayerId = iconSpriteRenderer.sortingLayerID;
      draggingSortingLayerId = SortingLayer.NameToID(DraggingSortingLayerName);
      Debug.AssertFormat(draggingSortingLayerId != 0, "ItemWithIconSprite behavior expects DraggingSortingLayerName to be valid sorting layer name.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemWithIconSprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemWithIconSprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      inventorySystem.ItemBeingDestroyed += InventorySystem_ItemBeingDestroyed;
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      iconSpriteRenderer.sprite = ItemBehaviour.ItemDatum.Sprite;
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        iconSpriteRenderer.sortingLayerID = draggingSortingLayerId;
      }

    }
    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        iconSpriteRenderer.sortingLayerID = defaultSortingLayerId;
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