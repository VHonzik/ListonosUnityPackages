using System;
using System.Collections.Generic;
using UnityEngine;

namespace Listonos.InvetorySystem
{
  public abstract class EnumKeyedDatum<EnumKey> where EnumKey : Enum
  {
    public EnumKey Key;
  }

  public abstract class StringKeyedDatum
  {
    public string Key;
  }

  public abstract class EnumKeyedSerializedData<Key, Datum>
    where Key : Enum
    where Datum : EnumKeyedDatum<Key>
  {
    public Datum[] Data;
    public Datum DefaultData;

    private Dictionary<Key, Datum> dataDict = new Dictionary<Key, Datum>();

    public void Init()
    {
      foreach (var visuals in Data)
      {
        dataDict.Add(visuals.Key, visuals);
      }
    }

    public Datum GetDatum(Key key)
    {
      Debug.AssertFormat(dataDict.ContainsKey(key), "Did not find datum for key {0} in EnumKeyedSerializedData.", key.ToString());
      if (!dataDict.ContainsKey(key))
      {
        return DefaultData;
      }

      return dataDict[key];
    }
  }

  public abstract class StringKeyedSerializedData<Datum> where Datum : StringKeyedDatum
  {
    public Datum[] Data;
    public Datum DefaultData;

    private Dictionary<string, Datum> dataDict = new Dictionary<string, Datum>();

    public void Init()
    {
      foreach (var visuals in Data)
      {
        dataDict.Add(visuals.Key, visuals);
      }
    }

    public Datum GetDatum(string key)
    {
      Debug.AssertFormat(dataDict.ContainsKey(key), "Did not find datum for key {0} in StringKeyedSerializedData.", key);
      if (!dataDict.ContainsKey(key))
      {
        return DefaultData;
      }

      return dataDict[key];
    }
  }

  public abstract class SlotDatum<SlotEnum> : EnumKeyedDatum<SlotEnum> where SlotEnum : Enum
  {
    public bool AllowAllItems;
    public Vector2Int Size;
  }

  public abstract class ItemQualityDatum<ItemQualityEnum> : EnumKeyedDatum<ItemQualityEnum> where ItemQualityEnum : Enum
  {
    public Sprite Sprite;
  }

  public abstract class ItemDatum<SlotEnum, ItemQualityEnum> : StringKeyedDatum
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
    public ItemQualityEnum ItemQuality;
    public Sprite Sprite;
    public bool HasItemSlot;
    public SlotEnum ItemSlot;
    public Vector2Int Size;
  }

  public abstract class ItemData<SlotEnum, ItemQualityEnum> : StringKeyedSerializedData<ItemDatum<SlotEnum, ItemQualityEnum>>
    where SlotEnum : Enum
    where ItemQualityEnum : Enum
  {
  }
}
