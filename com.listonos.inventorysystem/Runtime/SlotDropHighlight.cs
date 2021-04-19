using System;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public abstract class SlotDropHighlight<SlotEnum, ItemQualityEnum> : MonoBehaviour
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public GameObject DropHighlightSprite;
    public SlotBehaviour<SlotEnum, ItemQualityEnum> SlotBehavior;

    void Start()
    {
      Debug.AssertFormat(DropHighlightSprite != null, "SlotDropHighlight behavior expects valid reference to DropHighlightSprite game object.");
      Debug.AssertFormat(SlotBehavior != null, "SlotDropHighlight behavior expects valid reference to SlotBehavior.");

      SlotBehavior.DropHighlightChanged += SlotBehavior_DropHighlightChanged;

      DropHighlightSprite.SetActive(false);
    }

    private void SlotBehavior_DropHighlightChanged(object sender, SlotBehaviour<SlotEnum, ItemQualityEnum>.DropHighlightChangedEventArgs e)
    {
      DropHighlightSprite.SetActive(e.HighlightWanted);
    }
  }
}