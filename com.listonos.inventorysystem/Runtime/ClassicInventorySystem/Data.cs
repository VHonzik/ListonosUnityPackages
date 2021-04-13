using System;

namespace Listonos.InvetorySystem.Classic
{
  [Serializable]
  public class SlotDatum : SlotDatum<Slot>
  {
  }

  [Serializable]
  public class SlotData : EnumKeyedSerializedData<Slot, SlotDatum>
  {
  }

  [Serializable]
  public class ItemQualityDatum : ItemQualityDatum<ItemQuality>
  {
  }

  [Serializable]
  public class ItemQualityData : EnumKeyedSerializedData<ItemQuality, ItemQualityDatum>
  {
  }

  [Serializable]
  public class ItemDatum : ItemDatum<ItemQuality>
  {
  }

  [Serializable]
  public class ItemData : StringKeyedSerializedData<ItemDatum>
  {
  }
}