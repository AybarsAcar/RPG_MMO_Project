using RPG.Inventories;

namespace RPG.Shops
{
  public struct ShopItem
  {
    public InventoryItem Item;
    public int Availability;
    public float Price;
    public int QuantityInTransaction;
  }
}