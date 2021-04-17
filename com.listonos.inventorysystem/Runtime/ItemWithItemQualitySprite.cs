using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public class ItemWithItemQualitySprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject ItemQualitySprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;
    public bool ResizeSpriteToItemSize = true;

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer itemQualitySpriteRenderer;

    void Awake()
    {
      Debug.AssertFormat(ItemQualitySprite != null, "ItemWithItemQualitySprite behavior expects valid reference to ItemQualitySprite game object.");
      itemQualitySpriteRenderer = ItemQualitySprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(itemQualitySpriteRenderer != null, "ItemWithItemQualitySprite behavior expects ItemQualitySprite game object to have SpriteRenderer behavior.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemWithItemQualitySprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemWithItemQualitySprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
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
  }
}