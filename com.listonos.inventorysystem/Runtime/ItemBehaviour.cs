using System;
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
    public GameObject DetachParent;

    public Rigidbody2D RigidBody;

    public InventorySystem<SlotEnum, ItemQualityEnum> InventorySystem;

    private ItemDatum<ItemQualityEnum> itemDatum;
    private ItemQualityDatum<ItemQualityEnum> itemQualityDatum;

    private SpriteRenderer backgroundQualitySpriteRenderer;

    private Vector2 draggingOffset;

    // Start is called before the first frame update
    void Start()
    {
      Debug.AssertFormat(InventorySystem != null, "Slot behavior expects valid reference to InventorySystem behavior.");
      itemDatum = InventorySystem.GetItemDatum(Item);
      itemQualityDatum = InventorySystem.GetItemQualityDatum(itemDatum.ItemQuality);

      Debug.AssertFormat(IconSprite != null, "Item behavior expects valid reference to IconSprite game object.");
      Debug.AssertFormat(BackgroundQualitySprite != null, "Item behavior expects valid reference to BackgroundQualitySprite game object.");
      Debug.AssertFormat(DragHighlighSprite != null, "Item behavior expects valid reference to DragHighlighSprite game object.");

      IconSprite.SetActive(true);
      BackgroundQualitySprite.SetActive(true);
      backgroundQualitySpriteRenderer = BackgroundQualitySprite.GetComponent<SpriteRenderer>();
      backgroundQualitySpriteRenderer.sprite = itemQualityDatum.ItemBackgroundSprite;
      DragHighlighSprite.SetActive(false);

      RigidBody.simulated = false;
    }

    public void StartDragging(Vector3 mousePosition)
    {
      Vector2 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
      Vector2 ourWorldPosition = DetachParent.transform.position;
      draggingOffset = ourWorldPosition - worldMousePosition;
      DragHighlighSprite.SetActive(true);
      RigidBody.simulated = true;
    }

    public void StopDragging()
    {
      DragHighlighSprite.SetActive(false);
      DetachParent.transform.localPosition = Vector3.zero;
      RigidBody.simulated = false;
    } 

    public void Dragging(Vector3 mousePosition)
    {
      Vector2 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
      DetachParent.transform.position = worldMousePosition + draggingOffset;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
      var collidedObject = collision.gameObject;
      var collidingSlot = collidedObject.GetComponent<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
      if (collidingSlot)
      {
        InventorySystem.ItemBeginOverlapWithSlot(collidingSlot, this);
      }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
      var collidedObject = collision.gameObject;
      var collidingSlot = collidedObject.GetComponent<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
      if (collidingSlot)
      {
        InventorySystem.ItemStoppedOverlapWithSlot(collidingSlot, this);
      }
    }
  }
}
