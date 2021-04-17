using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public abstract class GridSlotCollection<SlotEnum, ItemQualityEnum> : MonoBehaviour, ISlotCollection<SlotEnum, ItemQualityEnum>
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public float SlotsDistance;
    public Vector2Int Dimensions;

    private Dictionary<SlotBehaviour<SlotEnum, ItemQualityEnum>, Vector2Int> slotsToIndices = new Dictionary<SlotBehaviour<SlotEnum, ItemQualityEnum>, Vector2Int>();
    private Dictionary<Vector2Int, SlotBehaviour<SlotEnum, ItemQualityEnum>> indicesToSlots = new Dictionary<Vector2Int, SlotBehaviour<SlotEnum, ItemQualityEnum>>();

    // Use this for initialization
    void Start()
    {
      Debug.Assert(Dimensions.x >= 1 || Dimensions.y >= 1);
      var childrenSlots = transform.GetComponentsInChildren<SlotBehaviour<SlotEnum, ItemQualityEnum>>().ToList();
      Debug.Assert(childrenSlots.Count == Dimensions.x * Dimensions.y);

      var epsilon = SlotsDistance * 0.5f;
      childrenSlots.Sort((slotA, slotB) =>
      {
        var slotAPosition = slotA.GetPosition();
        var slotBPosition = slotB.GetPosition();

        if (Mathf.Abs(slotAPosition.y - slotBPosition.y) < epsilon)
        {
          Debug.Assert(Mathf.Abs(slotAPosition.x - slotBPosition.x) > epsilon);
          return slotAPosition.x < slotBPosition.x ? -1 : 1;
        }
        else
        {
          return slotAPosition.y < slotBPosition.y ? 1 : -1;
        }
      });

      var index = 0;
      for (int y = 0; y < Dimensions.y; y++)
      {
        for (int x = 0; x < Dimensions.x; x++)
        {
          var indices = new Vector2Int(x, y);
          var slot = childrenSlots[index];
          slotsToIndices.Add(slot, indices);
          indicesToSlots.Add(indices, slot);
          slot.Collection = this;
          index++;
        }
      }
    }

    public List<SlotBehaviour<SlotEnum, ItemQualityEnum>> FilterSlotsPartOfCollection(IEnumerable<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots)
    {
      return slots.Where(slot => Contains(slot)).ToList();
    }

    public List<List<SlotBehaviour<SlotEnum, ItemQualityEnum>>> FindSlotsBlocksFittingDimensionsFromList(IEnumerable<SlotBehaviour<SlotEnum, ItemQualityEnum>> slots, Vector2Int dimensions)
    {
      Debug.Assert(dimensions.x > 1 || dimensions.y > 1);
      var result = new List<List<SlotBehaviour<SlotEnum, ItemQualityEnum>>>();
      var slotsSet = new HashSet<SlotBehaviour<SlotEnum, ItemQualityEnum>>(slots);

      foreach (var slot in slots)
      {
        Debug.Assert(Contains(slot));
        var slotIndices = slotsToIndices[slot];

        var block = new List<SlotBehaviour<SlotEnum, ItemQualityEnum>>();
        block.Add(slot);
        var keepGoing = true;
        for (int x = 0; x < dimensions.x; x++)
        {
          if (!keepGoing)
          {
            break;
          }

          for (int y = 0; y < dimensions.y; y++)
          {
            if (x== 0 && y == 0)
            {
              continue;
            }

            if (!keepGoing)
            {
              break;
            }

            var offset = new Vector2Int(x, y);
            var wantedIndices = slotIndices + offset;
            var found = false;

            if (AreIndicesValid(slotIndices + offset))
            {
              var wantedSlot = indicesToSlots[wantedIndices];
              if (slotsSet.Contains(wantedSlot))
              {
                block.Add(wantedSlot);
                found = true;
              }
            }

            if (!found)
            {
              keepGoing = false;
              break;
            }
          }
        }

        if (keepGoing)
        {
          result.Add(block);
        }
      }

      return result;
    }

    public bool Contains(SlotBehaviour<SlotEnum, ItemQualityEnum> slot)
    {
      return ReferenceEquals(slot.Collection, this);
    }

    private bool AreIndicesValid(Vector2Int indices)
    {
      return indices.x >= 0 && indices.x < Dimensions.x && indices.y >= 0 && indices.y < Dimensions.y;
    }
  }
}