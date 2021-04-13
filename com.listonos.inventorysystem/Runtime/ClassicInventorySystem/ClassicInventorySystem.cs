namespace Listonos.InvetorySystem.Classic
{
  public class ClassicInventorySystem : InventorySystem<Slot, ItemQuality>
  {
    public SlotData SlotData;
    public ItemQualityData ItemQualityData;
    public ItemData ItemData;

    public virtual void Awake()
    {
      SlotData.Init();
      ItemQualityData.Init();
      ItemData.Init();
    }

    public override ItemDatum<ItemQuality> GetItemDatum(string item)
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
  }
}
