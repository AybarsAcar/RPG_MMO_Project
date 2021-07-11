using RPG.Inventories;

namespace RPG.Shops
{
  public struct ShopItem
  {
    public InventoryItem InventoryItem;
    public int Availability;
    public float Price;
    public int QuantityInTransaction;
  }
}