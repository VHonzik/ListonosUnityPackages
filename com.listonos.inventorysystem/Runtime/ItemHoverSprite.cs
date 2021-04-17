using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public class ItemHoverSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject HoverSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> ItemBehaviour;

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;

    void Start()
    {
      Debug.AssertFormat(HoverSprite != null, "ItemHoverSprite behavior expects valid reference to HoverSprite game object.");
      Debug.AssertFormat(ItemBehaviour != null, "ItemHoverSprite behavior expects valid reference to ItemBehavior.");

      inventorySystem = ItemBehaviour.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "ItemHoverSprite behavior did not find InventorySystem on ItemBehaviour.");

      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;

      // TODO add support for scaling with item size

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
  }
}