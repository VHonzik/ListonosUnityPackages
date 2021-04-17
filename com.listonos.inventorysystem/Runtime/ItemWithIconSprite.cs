using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public class ItemWithIconSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject IconSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;
    private SpriteRenderer iconSpriteRenderer;

    void Awake()
    {
      Debug.AssertFormat(IconSprite != null, "ItemWithIconSprite behavior expects valid reference to IconSprite game object.");
      iconSpriteRenderer = IconSprite.GetComponent<SpriteRenderer>();
      Debug.AssertFormat(iconSpriteRenderer != null, "ItemWithIconSprite behavior expects IconSprite game object to have SpriteRenderer behavior.");

      Debug.AssertFormat(ItemBehaviour != null, "ItemWithIconSprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemWithIconSprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.AfterDataReady += InventorySystem_AfterDataReady;
    }

    private void InventorySystem_AfterDataReady(object sender, EventArgs e)
    {
      iconSpriteRenderer.sprite = ItemBehaviour.ItemDatum.Sprite;
    }
  }
}