using System;
using System.Collections.Generic;
using UnityEngine;

namespace Listonos.InventorySystem
{
  public interface ISlotCollection<SlotEnum, ItemQualityEnum>
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public bool Contains(SlotBehaviour<SlotEnum, ItemQualityEnum> slot);
    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> FilterSlotsPartOfCollection(IEnumerable<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots);
    public List<List<SlotBehaviour<SlotEnum, ItemQualityEnum>>> FindSlotsBlocksFittingDimensionsFromList(IEnumerable<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots, Vector2Int dimensions);
  }
}
