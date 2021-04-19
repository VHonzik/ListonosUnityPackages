using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public abstract class ItemBehaviour<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public string Item;
    public Rigidbody2D Rigidbody;
    public InventorySystem<SlotEnum, ItemQualityEnum> InventorySystem;

    public ItemDatum<SlotEnum, ItemQualityEnum> ItemDatum { get; private set; }
    public int Stacks { get; private set; } = 1;

    private Vector2 draggingOffset;
    private Vector3 draggingStartPosition;

    void Awake()
    {
      Debug.AssertFormat(InventorySystem != null, "Slot behavior expects valid reference to InventorySystem behavior.");
      InventorySystem.DataReady += InventorySystem_DataReady;
      InventorySystem.ItemStartedDragging += InventorySystem_ItemStartedDragging;
      InventorySystem.ItemStoppedDragging += InventorySystem_ItemStoppedDragging;
      InventorySystem.ItemBeingDestroyed += InventorySystem_ItemBeingDestroyed;
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
    }

    public void StartDragging(Vector3 mousePosition)
    {
      draggingStartPosition = transform.position;
      Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
      Vector2 ourWorldPosition = transform.position;
      draggingOffset = ourWorldPosition - worldMousePosition;
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
    public Vector2 GetPosition()
    {
      return transform.position;
    }

    public void IncreaseStacks(int amout)
    {
      Stacks += amout;
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

    private void InventorySystem_DataReady(object sender, EventArgs e)
    {
      ItemDatum = InventorySystem.GetItemDatum(Item);
    }

    private void InventorySystem_ItemBeingDestroyed(object sender, InventorySystem<SlotEnum, ItemQualityEnum>.ItemBeingDestroyedEventArgs e)
    {
      if (ReferenceEquals(e.ItemBehaviour, this))
      {
        InventorySystem.DataReady -= InventorySystem_DataReady;
        InventorySystem.ItemStartedDragging -= InventorySystem_ItemStartedDragging;
        InventorySystem.ItemStoppedDragging -= InventorySystem_ItemStoppedDragging;
        InventorySystem.ItemBeingDestroyed -= InventorySystem_ItemBeingDestroyed;
      }
    }
  }
}
