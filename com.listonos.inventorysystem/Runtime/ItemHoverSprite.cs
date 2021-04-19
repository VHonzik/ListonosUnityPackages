using System;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public class ItemHoverSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject HoverSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public bool ResizeSpriteToItemSize = true;
    public float SizeAddition = 0.2f;

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer hoverSpriteRenderer;

    void Awake()
    {
      Debug.AssertFormat(HoverSprite != null, "ItemHoverSprite behavior expects valid reference to HoverSprite game object.");
      hoverSpriteRenderer = HoverSprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(hoverSpriteRenderer != null, "ItemHoverSprite behavior expects HoverSprite game object to have SpriteRenderer behavior.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemHoverSprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemHoverSprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
      inventorySystem.ItemBeingDestroyed += InventorySystem_ItemBeingDestroyed;
    }

    void Start()
    {
      HoverSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
      if (!inventorySystem.DraggingItem)
      {
        HoverSprite.SetActive(true);
      }
    }

    void OnMouseExit()
    {
      if (!inventorySystem.DraggingItem)
      {
        HoverSprite.SetActive(false);
      }
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        HoverSprite.SetActive(false);
      }

    }
    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, ItemBehaviour))
      {
        HoverSprite.SetActive(true);
      }
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      if (ResizeSpriteToItemSize)
      {
        Debug.AssertFormat(hoverSpriteRenderer.drawMode == SpriteDrawMode.Sliced, "ItemHoverSprite behavior has ResizeSpriteToItemSize set to true but the HoverSprite sprite renderer is not set to SpriteDrawMode.Sliced");
        hoverSpriteRenderer.size = ItemBehaviour.ItemDatum.Size + Vector2.one * SizeAddition;
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