using System;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class ItemWithItemQualitySprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject ItemQualitySprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public bool ResizeSpriteToItemSize = true;
    public string DraggingSortingLayerName = "Default";

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer itemQualitySpriteRenderer;
    private int defaultSortingLayerId;
    private int draggingSortingLayerId;

    void Awake()
    {
      Debug.AssertFormat(ItemQualitySprite != null, "ItemWithItemQualitySprite behavior expects valid reference to ItemQualitySprite game object.");
      itemQualitySpriteRenderer = ItemQualitySprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(itemQualitySpriteRenderer != null, "ItemWithItemQualitySprite behavior expects ItemQualitySprite game object to have SpriteRenderer behavior.");
      defaultSortingLayerId = itemQualitySpriteRenderer.sortingLayerID;
      draggingSortingLayerId = SortingLayer.NameToID(DraggingSortingLayerName);
      Debug.AssertFormat(draggingSortingLayerId != 0, "ItemWithItemQualitySprite behavior expects DraggingSortingLayerName to be valid sorting layer name.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemWithItemQualitySprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemWithItemQualitySprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      inventorySystem.ItemBeingDestroyed += InventorySystem_ItemBeingDestroyed;
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      if (ResizeSpriteToItemSize)
      {
        Debug.AssertFormat(itemQualitySpriteRenderer.drawMode == SpriteDrawMode.Sliced, "ItemWithItemQualitySprite behavior has ResizeSpriteToItemSize set to true but the ItemQualitySprite sprite renderer is not set to SpriteDrawMode.Sliced");
        itemQualitySpriteRenderer.size = ItemBehaviour.ItemDatum.Size;
      }

      itemQualitySpriteRenderer.sprite = inventorySystem.GetItemQualityDatum(ItemBehaviour.ItemDatum.ItemQuality).Sprite;
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        itemQualitySpriteRenderer.sortingLayerID = draggingSortingLayerId;
      }

    }
    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        itemQualitySpriteRenderer.sortingLayerID = defaultSortingLayerId;
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