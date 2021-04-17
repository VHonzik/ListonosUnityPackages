using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public class SlotWithSlotSprite<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public SpriteRenderer SlotSpriteRenderer;
    public SlotBehaviour<SlotEnum, ItemQualityEnum> SlotBehavior;
    public Sprite NormalSprite;
    public Sprite DisabledSprite;

    private InventorySystem<SlotEnum, ItemQualityEnum> inventorySystem;

    void Start()
    {
      Debug.AssertFormat(SlotSpriteRenderer != null, "SlotWithSlotSprite behavior expects valid reference to SlotSpriteRenderer.");
      Debug.AssertFormat(SlotBehavior != null, "SlotWithSlotSprite behavior expects valid reference to SlotBehavior.");
      Debug.AssertFormat(NormalSprite != null, "SlotWithSlotSprite behavior expects valid reference to NormalSprite.");
      Debug.AssertFormat(DisabledSprite != null, "SlotWithSlotSprite behavior expects valid reference to DisabledSprite.");
      inventorySystem = SlotBehavior.InventorySystem;
      Debug.AssertFormat(inventorySystem != null, "SlotWithSlotSprite behavior did not find InventorySystem on SlotBehavior.");

      SlotSpriteRenderer.sprite = NormalSprite;
      inventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      inventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (!inventorySystem.ItemCanGoIntoSlot(e.ItemBehaviour, SlotBehavior))
      {
        SlotSpriteRenderer.sprite = DisabledSprite;
      }
    }

    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      SlotSpriteRenderer.sprite = NormalSprite;
    }
  }
}