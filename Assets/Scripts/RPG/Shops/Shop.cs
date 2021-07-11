using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Control;
using RPG.Core.Util;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
  /// <summary>
  /// on the shop keeper or the object that requires to behave like the shop
  /// this class also determines which items a shop will have
  /// </summary>
  public class Shop : MonoBehaviour, IRaycastable
  {
    [Serializable]
    private struct StockConfig
    {
      public InventoryItem InventoryItem;
      public int InitialStock;
      [Range(0, 100)] public float BuyingDiscountPercentage;
    }

    [SerializeField] private StockConfig[] stockConfigs;

    [SerializeField] private string shopName;
    public string ShopName => shopName;

    private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
    private Shopper _currentShopper;

    public event Action ONChange;

    public void SetShopper(Shopper shopper)
    {
      _currentShopper = shopper;
    }

    public IEnumerable<ShopItem> GetFilteredItems()
    {
      return GetAllItems();
    }

    /// <summary>
    /// returns all the items in the shop keepers prefab
    /// </summary>
    /// <returns>Shop items as enumerable</returns>
    public IEnumerable<ShopItem> GetAllItems()
    {
      foreach (var config in stockConfigs)
      {
        _transaction.TryGetValue(config.InventoryItem, out var quantityInTransaction);

        yield return new ShopItem
        {
          InventoryItem = config.InventoryItem,
          Availability = config.InitialStock,
          Price = config.InventoryItem.Price * (1 - (config.BuyingDiscountPercentage / 100)),
          QuantityInTransaction = quantityInTransaction,
        };
      }
    }

    public void SelectFilter(ItemCategory category)
    {
    }

    public ItemCategory GetFilter()
    {
      return ItemCategory.None;
    }

    public void SelectMode(bool isBuying)
    {
    }

    public bool IsBuyingMode()
    {
      return true;
    }

    public bool CanTransact()
    {
      return true;
    }

    public float GetTransactionTotal()
    {
      var total = 0f;

      foreach (var shopItem in GetAllItems())
      {
        total += shopItem.Price * shopItem.QuantityInTransaction;
      }

      return total;
    }

    /// <summary>
    /// method to increment or decrement to the quantity
    /// updates the _transaction dictionary
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
    public void AddToTransaction(InventoryItem item, int quantity)
    {
      if (!_transaction.ContainsKey(item))
      {
        _transaction[item] = 0;
      }

      _transaction[item] += quantity;

      // so the quantity won't go below 0
      if (_transaction[item] <= 0)
      {
        _transaction.Remove(item);
      }

      // update the state
      ONChange?.Invoke();
    }

    /// <summary>
    /// confirms selling an buying transactions
    /// called from sell / buy button in our shopUI
    /// </summary>
    public void ConfirmTransaction()
    {
      var shopperInventory = _currentShopper.GetComponent<Inventory>();
      var playerBalance = _currentShopper.GetComponent<PlayerBalance>();
      if (shopperInventory == null || playerBalance == null) return;

      // Transfer to or from the inventory
      foreach (var shopItem in GetAllItems())
      {
        var inventoryItem = shopItem.InventoryItem;
        var quantity = shopItem.QuantityInTransaction;
        var price = shopItem.Price;

        // to prevent non-stackable item from stacking
        for (int i = 0; i < quantity; i++)
        {
          if (playerBalance.CurrentBalance < price)
          {
            break;
          }

          var success = shopperInventory.AddToFirstEmptySlot(inventoryItem, 1);

          if (success)
          {
            // remove from transaction
            AddToTransaction(inventoryItem, -1);

            // Debiting or crediting of funds
            playerBalance.UpdateBalance(-price);
          }
        }
      }

      // clear the transaction as a safe
      _transaction.Clear();
    }

    public bool HandleRaycast(PlayerController callingController)
    {
      if (Input.GetMouseButtonDown(0) /*&& IsInInteractionDistance()*/)
      {
        callingController.GetComponent<Shopper>().SetActiveShop(this);
      }

      return true;
    }

    public CursorType GetCursorType()
    {
      // if (IsInInteractionDistance())
      // {
      return CursorType.Shop;
      // }

      // return CursorType.None;
    }

    private bool IsInInteractionDistance()
    {
      var player = GameObject.FindGameObjectWithTag(Tag.Player);

      var distance = Vector3.Distance(player.transform.position, transform.position);


      return distance < 2f;
    }
  }
}