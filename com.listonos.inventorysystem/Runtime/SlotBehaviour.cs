
using System;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public abstract class SlotBehaviour<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public SlotEnum Slot;
    public InventorySystem<SlotEnum, ItemQualityEnum> InventorySystem;

    public GameObject SlotTypeSprite;
    public GameObject DropHighlightSprite;
    public Rigidbody2D Rigidbody;

    private SpriteRenderer slotTypeSpriteRenderer;
    private SlotDatum<SlotEnum> slotDatum;

    void Start()
    {
      Debug.AssertFormat(InventorySystem != null, "Slot behavior expects valid reference to InventorySystem behavior.");
      InventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      InventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      slotDatum = InventorySystem.GetSlotDatum(Slot);

      Debug.AssertFormat(SlotTypeSprite != null, "Slot behavior expects valid reference to SlotTypeSprite game object.");
      Debug.AssertFormat(DropHighlightSprite != null, "Slot behavior expects valid reference to DropHighlightSprite game object.");

      if (slotDatum.ShowSprite)
      {
        SlotTypeSprite.SetActive(true);
        slotTypeSpriteRenderer = SlotTypeSprite.GetComponent<SpriteRenderer>();
        slotTypeSpriteRenderer.sprite = slotDatum.NormalSprite;
      }
      else
      {
        SlotTypeSprite.SetActive(false);
      }

      DropHighlightSprite.SetActive(false);
      Rigidbody.simulated = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
      var collidedObject = collision.gameObject;
      var collidingItem = collidedObject.GetComponent<ItemBehaviour<SlotEnum, ItemQualityEnum>>();
      if (collidingItem)
      {
        InventorySystem.ItemBeginOverlapWithSlot(this,  collidingItem);
      }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
      var collidedObject = collision.gameObject;
      var collidingItem = collidedObject.GetComponent<ItemBehaviour<SlotEnum, ItemQualityEnum>>();
      if (collidingItem)
      {
        InventorySystem.ItemStoppedOverlapWithSlot(this, collidingItem);
      }
    }

    public void ItemEnteredDrop()
    {
      DropHighlightSprite.SetActive(true);
    }

    public void ItemExitedDrop()
    {
      DropHighlightSprite.SetActive(false);
    }

    public bool AcceptsItem(ItemDatum<SlotEnum, ItemQualityEnum> itemDatum)
    {
      return slotDatum.AllowAllItems || (!slotDatum.AllowAllItems && itemDatum.HasItemSlot && Equals(Slot, itemDatum.ItemSlot));
    }

    public bool AcceptsItem(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      return AcceptsItem(item.ItemDatum);
    }

    public void RemoveItem()
    {
      DropHighlightSprite.SetActive(false);
    }

    public void DropItem(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      DropHighlightSprite.SetActive(false);
      item.transform.position = transform.position;
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      Rigidbody.simulated = true;
      if (slotDatum.ShowSprite && !AcceptsItem(e.ItemBehaviour))
      {
        slotTypeSpriteRenderer.sprite = slotDatum.DisabledSprite;
      }
    }

    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      Rigidbody.simulated = false;
      if (slotDatum.ShowSprite && !AcceptsItem(e.ItemBehaviour))
      {
        slotTypeSpriteRenderer.sprite = slotDatum.NormalSprite;
      }

    }
  }
}
