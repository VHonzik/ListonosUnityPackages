using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public abstract class ItemBehaviour<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public string Item;

    public GameObject IconSprite;
    public GameObject BackgroundQualitySprite;
    public GameObject DragHighlighSprite;
    public GameObject HoverSprite;
    public Rigidbody2D Rigidbody;

    public float HoverSpriteSizeAddition;

    public InventorySystem<SlotEnum, ItemQualityEnum> InventorySystem;

    public ItemDatum<SlotEnum, ItemQualityEnum> ItemDatum { get; private set; }
    private ItemQualityDatum<ItemQualityEnum> itemQualityDatum;

    private SpriteRenderer backgroundQualitySpriteRenderer;

    private Vector2 draggingOffset;
    private Vector3 draggingStartPosition;

    // Start is called before the first frame update
    void Start()
    {
      Debug.AssertFormat(InventorySystem != null, "Slot behavior expects valid reference to InventorySystem behavior.");
      ItemDatum = InventorySystem.GetItemDatum(Item);
      itemQualityDatum = InventorySystem.GetItemQualityDatum(ItemDatum.ItemQuality);

      InventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      InventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;

      Debug.AssertFormat(IconSprite != null, "Item behavior expects valid reference to IconSprite game object.");
      Debug.AssertFormat(BackgroundQualitySprite != null, "Item behavior expects valid reference to BackgroundQualitySprite game object.");
      Debug.AssertFormat(DragHighlighSprite != null, "Item behavior expects valid reference to DragHighlighSprite game object.");
      Debug.AssertFormat(HoverSprite != null, "Slot behavior expects valid reference to HoverSprite game object.");

      IconSprite.SetActive(true);
      BackgroundQualitySprite.SetActive(true);
      backgroundQualitySpriteRenderer = BackgroundQualitySprite.GetComponent<SpriteRenderer>();
      backgroundQualitySpriteRenderer.sprite = itemQualityDatum.ItemBackgroundSprite;
      backgroundQualitySpriteRenderer.size = ItemDatum.Size;
      var dragHighlighSpriteRenderer = DragHighlighSprite.GetComponent<SpriteRenderer>();
      dragHighlighSpriteRenderer.size = ItemDatum.Size;
      DragHighlighSprite.SetActive(false);
      var hoverSpriteRenderer = HoverSprite.GetComponent<SpriteRenderer>();
      hoverSpriteRenderer.size = new Vector2(ItemDatum.Size.x, ItemDatum.Size.y) + Vector2.one * HoverSpriteSizeAddition;
      HoverSprite.SetActive(false);
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
      InventorySystem.StartDraggingItem(this);
      StartDragging(Input.mousePosition);
    }

    void OnMouseDrag()
    {
      Dragging(Input.mousePosition);
    }

    void OnMouseUp()
    {
      InventorySystem.StopDraggingItem(this);
      StopDragging();
    }

    public void StartDragging(Vector3 mousePosition)
    {
      HoverSprite.SetActive(false);
      draggingStartPosition = transform.position;
      Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
      Vector2 ourWorldPosition = transform.position;
      draggingOffset = ourWorldPosition - worldMousePosition;
      DragHighlighSprite.SetActive(true);
    }

    public void StopDragging()
    {
      DragHighlighSprite.SetActive(false);
    }

    public void Dragging(Vector3 mousePosition)
    {
      Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
      transform.position = worldMousePosition + draggingOffset;
    }

    public void ResetPositionToStartDrag()
    {
      transform.position = draggingStartPosition;
    }

    public void SetPositionToSlots(List<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots)
    {
      var averagePosition = slots.Aggregate(Vector3.zero, (positionSum, slot) =>
      {
        return positionSum + slot.transform.position;
      });
      transform.position = averagePosition / slots.Count;
    }
    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (e.ItemBehaviour != this)
      {
        Rigidbody.simulated = false;
      }
    }

    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      if (e.ItemBehaviour != this)
      {
        Rigidbody.simulated = true;
      }
    }
  }
}
