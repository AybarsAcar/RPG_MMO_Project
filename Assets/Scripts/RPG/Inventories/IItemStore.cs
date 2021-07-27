namespace RPG.Inventories
{
  public interface IItemStore
  {
    /// <summary>
    /// returns the number of items it took
    /// </summary>
    /// <param name="item"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    int AddItems(InventoryItem item, int number);
  }
}