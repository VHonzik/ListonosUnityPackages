using System;

namespace Listonos.InventorySystem.Classic
{
  public class InventorySystem : InventorySystem<Slot, ItemQuality>
  {
    public SlotData SlotData;
    public ItemQualityData ItemQualityData;
    public ItemData ItemData;

    public override ItemDatum<Slot, ItemQuality> GetItemDatum(string item)
    {
      return ItemData.GetDatum(item);
    }

    public override ItemQualityDatum<ItemQuality> GetItemQualityDatum(ItemQuality itemQuality)
    {
      return ItemQualityData.GetDatum(itemQuality);
    }

    public override SlotDatum<Slot> GetSlotDatum(Slot slot)
    {
      return SlotData.GetDatum(slot);
    }

    public override void LoadData()
    {
      SlotData.Init();
      ItemQualityData.Init();
      ItemData.Init();
    }
  }
}
