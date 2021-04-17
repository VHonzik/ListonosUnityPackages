using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public class ItemDragHighlightSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject DragHighlightSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public bool ResizeSpriteToItemSize = true;

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer hoverSpriteRenderer;

    void Awake()
    {
      Debug.AssertFormat(DragHighlightSprite != null, "ItemDragHighlightSprite behavior expects valid reference to DragHighlightSprite game object.");
      hoverSpriteRenderer = DragHighlightSprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(hoverSpriteRenderer != null, "ItemDragHighlightSprite behavior expects DragHighlightSprite game object to have SpriteRenderer behavior.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemDragHighlightSprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemDragHighlightSprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
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
      }

    }
    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        DragHighlightSprite.SetActive(false);
      }
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      if (ResizeSpriteToItemSize)
      {
        Debug.AssertFormat(hoverSpriteRenderer.drawMode == SpriteDrawMode.Sliced, "ItemDragHighlightSprite behavior has ResizeSpriteToItemSize set to true but the DragHighlightSprite sprite renderer is not set to SpriteDrawMode.Sliced");
        hoverSpriteRenderer.size = ItemBehaviour.ItemDatum.Size;
      }
    }
  }
}