using System;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class ItemDragHighlightSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject DragHighlightSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public bool ResizeSpriteToItemSize = true;
    public string DraggingSortingLayerName = "Default";

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer dragHighlightSpriteRenderer;
    private int defaultSortingLayerId;
    private int draggingSortingLayerId;

    void Awake()
    {
      Debug.AssertFormat(DragHighlightSprite != null, "ItemDragHighlightSprite behavior expects valid reference to DragHighlightSprite game object.");
      dragHighlightSpriteRenderer = DragHighlightSprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(dragHighlightSpriteRenderer != null, "ItemDragHighlightSprite behavior expects DragHighlightSprite game object to have SpriteRenderer behavior.");
      defaultSortingLayerId = dragHighlightSpriteRenderer.sortingLayerID;
      draggingSortingLayerId = SortingLayer.NameToID(DraggingSortingLayerName);
      Debug.AssertFormat(draggingSortingLayerId != 0, "ItemDragHighlightSprite behavior expects DraggingSortingLayerName to be valid sorting layer name.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemDragHighlightSprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemDragHighlightSprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      inventorySystem.ItemBeingDestroyed += InventorySystem_ItemBeingDestroyed;
    }

    void Start()
    {
      DragHighlightSprite.SetActive(false);
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        DragHighlightSprite.SetActive(true);
        dragHighlightSpriteRenderer.sortingLayerID = draggingSortingLayerId;
      }

    }
    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        DragHighlightSprite.SetActive(false);
        dragHighlightSpriteRenderer.sortingLayerID = defaultSortingLayerId;
      }
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      if (ResizeSpriteToItemSize)
      {
        Debug.AssertFormat(dragHighlightSpriteRenderer.drawMode == SpriteDrawMode.Sliced, "ItemDragHighlightSprite behavior has ResizeSpriteToItemSize set to true but the DragHighlightSprite sprite renderer is not set to SpriteDrawMode.Sliced");
        dragHighlightSpriteRenderer.size = ItemBehaviour.ItemDatum.Size;
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