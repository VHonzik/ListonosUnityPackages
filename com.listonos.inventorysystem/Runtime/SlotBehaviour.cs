
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

    public GameObject HoverSprite;
    public GameObject SlotTypeSprite;
    public GameObject DropHighlightSprite;
    public ItemBehaviour<SlotEnum, ItemQualityEnum> Item;

    private SpriteRenderer slotTypeSpriteRenderer;
    private SlotDatum<SlotEnum> slotDatum;

    void Start()
    {
      Debug.AssertFormat(InventorySystem != null, "Slot behavior expects valid reference to InventorySystem behavior.");
      InventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      slotDatum = InventorySystem.GetSlotDatum(Slot);

      Debug.AssertFormat(HoverSprite != null, "Slot behavior expects valid reference to HoverSprite game object.");
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
      
      HoverSprite.SetActive(false);
      DropHighlightSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
      if (!InventorySystem.DraggingItem)
      {
        HoverSprite.SetActive(true);
      }      
    }

    void OnMouseExit()
    {
      if (!InventorySystem.DraggingItem)
      {
        HoverSprite.SetActive(false);
      }
    }

    void OnMouseDown()
    {
      if (Item)
      {
        InventorySystem.StartDraggingItem(this, Item);
      }
    }

    void OnMouseDrag()
    {
      if (Item)
      {
        InventorySystem.ContinueDraggingItem(this, Item);
      }
    }

    void OnMouseUp()
    {
      if (Item)
      {
        InventorySystem.StopDraggingItem(this, Item);
      }
    }

    public void ItemEnteredDrop(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      DropHighlightSprite.SetActive(true);
    }

    public void ItemExitedDrop(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      DropHighlightSprite.SetActive(false);
    }

    public void RemoveItem(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      DropHighlightSprite.SetActive(false);
      Debug.Assert(Item == item);
      Item = null;
    }

    public void DropItem(ItemBehaviour<SlotEnum, ItemQualityEnum> item)
    {
      DropHighlightSprite.SetActive(false);
      item.transform.position = transform.position;
      Item = item;
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (e.sourceSlotBehavior == this)
      {
        HoverSprite.SetActive(false);
      }
    }
  }
}
