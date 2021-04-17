
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
    public Rigidbody2D Rigidbody;

    public SlotDatum<SlotEnum> SlotDatum { get; private set; }
    public ISlotCollection<SlotEnum, ItemQualityEnum> Collection { get; set; } = null;

    public class DropHighlightChangedEventArgs : EventArgs
    {
      public bool HighlightWanted { get; set; }
    }

    public event EventHandler<DropHighlightChangedEventArgs> DropHighlightChanged;

    void Awake()
    {
      Debug.AssertFormat(InventorySystem != null, "Slot behavior expects valid reference to InventorySystem behavior.");
      InventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      InventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      InventorySystem.DataReady += InventorySystem_DataReady;
    }

    void Start()
    {
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

    public void ShowDropHighlight(object sender)
    {
      DropHighlightChanged?.Invoke(sender, new DropHighlightChangedEventArgs() { HighlightWanted = true });
    }

    public void HideDropHighlight(object sender)
    {
      DropHighlightChanged?.Invoke(sender, new DropHighlightChangedEventArgs() { HighlightWanted = false });
    }

    public Vector2 GetPosition()
    {
      return transform.position;
    }

    private void InventorySystem_ItemStartedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      Rigidbody.simulated = true;
    }

    private void InventorySystem_ItemStoppedDragging(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemDragEventArgs e)
    {
      Rigidbody.simulated = false;
    }

    private void InventorySystem_DataReady(object sender, EventArgs e)
    {
      SlotDatum = InventorySystem.GetSlotDatum(Slot);
    }
  }
}
