using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Control;
using RPG.Core.Util;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shops
{
  /// <summary>
  /// on the shop keeper or the object that requires to behave like the shop
  /// this class also determines which items a shop will have
  /// </summary>
  public class Shop : MonoBehaviour, IRaycastable, ISavable
  {
    [Serializable]
    private class StockConfig
    {
      public InventoryItem InventoryItem; // our Scriptable object
      public int InitialStock;
      [Range(0, 100)] public float BuyingDiscountPercentage;
      public int LevelToUnlock = 0;
    }

    // so the player will be able to sell the item for the 60% of its original price
    [Tooltip("The value is a fraction")] [SerializeField] [Range(0, 1)]
    private float sellingRate = 0.6f;

    [SerializeField] private StockConfig[] stockConfigs;

    [SerializeField] private string shopName;
    public string ShopName => shopName;

    private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();

    // keeps track of the shop stock
    private Dictionary<InventoryItem, int> _stockSold = new Dictionary<InventoryItem, int>();

    private Shopper _currentShopper;

    private bool _isBuyingMode = true;
    public bool IsBuyingMode => _isBuyingMode;

    private ItemCategory filter = ItemCategory.None;
    public ItemCategory Filter => filter;

    public event Action ONChange;


    public void SetShopper(Shopper shopper)
    {
      _currentShopper = shopper;
    }

    /// <summary>
    /// returns the items if the inventory item category equals to the selected filter
    /// or returns all if filter == ItemCategory.None
    /// used to display the RowUI based  on the filters selected
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ShopItem> GetFilteredItems()
    {
      return GetAllItems().Where(item => filter == ItemCategory.None || filter == item.InventoryItem.Category);
    }

    /// <summary>
    /// returns all the items in the shop keepers prefab
    /// </summary>
    /// <returns>Shop items as enumerable</returns>
    public IEnumerable<ShopItem> GetAllItems()
    {
      var pricesDictionary = GetPrices();
      var availabilitiesDictionary = GetAvailabilities();

      foreach (var inventoryItem in availabilitiesDictionary.Keys)
      {
        if (availabilitiesDictionary[inventoryItem] <= 0) continue;

        _transaction.TryGetValue(inventoryItem, out var quantityInTransaction);

        yield return new ShopItem
        {
          InventoryItem = inventoryItem,
          Availability = availabilitiesDictionary[inventoryItem],
          Price = pricesDictionary[inventoryItem],
          QuantityInTransaction = quantityInTransaction,
        };
      }
    }

    private Dictionary<InventoryItem, int> GetAvailabilities()
    {
      var availabilities = new Dictionary<InventoryItem, int>();

      foreach (var config in GetAvailableConfigs())
      {
        if (_isBuyingMode)
        {
          if (!availabilities.ContainsKey(config.InventoryItem))
          {
            _stockSold.TryGetValue(config.InventoryItem, out var sold);
            availabilities[config.InventoryItem] = -sold;
          }

          availabilities[config.InventoryItem] += config.InitialStock;
        }
        else
        {
          // selling mode
          // return the inventory items
          availabilities[config.InventoryItem] = CountItemsInInventory(config.InventoryItem);
        }
      }

      return availabilities;
    }

    private Dictionary<InventoryItem, float> GetPrices()
    {
      var prices = new Dictionary<InventoryItem, float>();

      foreach (var config in GetAvailableConfigs())
      {
        if (_isBuyingMode)
        {
          if (!prices.ContainsKey(config.InventoryItem))
          {
            prices[config.InventoryItem] = config.InventoryItem.Price;
          }

          prices[config.InventoryItem] *= (1 - config.BuyingDiscountPercentage / 100);
        }
        else
        {
          // selling case
          // simple doesn't depend on the level
          prices[config.InventoryItem] = config.InventoryItem.Price * sellingRate;
        }
      }

      return prices;
    }

    private IEnumerable<StockConfig> GetAvailableConfigs()
    {
      var shopperLevel = GetShopperLevel();
      foreach (var config in stockConfigs)
      {
        if (config.LevelToUnlock <= shopperLevel)
        {
          yield return config;
        }
      }
    }

    private int CountItemsInInventory(InventoryItem inventoryItem)
    {
      var playerInventory = _currentShopper.GetComponent<Inventory>();
      if (playerInventory == null) return 0;

      var count = 0;
      for (int i = 0; i < playerInventory.GetSize(); i++)
      {
        if (inventoryItem == playerInventory.GetItemInSlot(i))
        {
          count += playerInventory.GetNumberInSlot(i);
        }
      }

      return count;
    }

    public void SelectFilter(ItemCategory category)
    {
      filter = category;
      ONChange?.Invoke();
    }

    /// <summary>
    /// this method is basically a setter for _isBuyingMode
    /// plus it broadcasts the ONChange events
    /// </summary>
    /// <param name="isBuying"></param>
    public void SelectMode(bool isBuying)
    {
      _isBuyingMode = isBuying;
      ONChange?.Invoke();
    }

    /// <summary>
    /// used to disable the button and give errors when we can't proceed with the transaction
    /// </summary>
    /// <returns></returns>
    public bool CanTransact()
    {
      if (!_isBuyingMode) return true;

      // Empty transaction
      if (IsTransactionEmpty()) return false;

      // Not sufficient funds
      if (!HasSufficientFunds()) return false;

      if (!HasInventorySpace()) return false;

      // Not sufficient inventory space
      return true;
    }

    /// <summary>
    /// Checks whether the player have enough slots available in their inventory
    /// to ConfirmTransaction
    /// flattens the list
    /// </summary>
    /// <returns></returns>
    private bool HasInventorySpace()
    {
      if (!_isBuyingMode) return true;

      var playerInventory = _currentShopper.GetComponent<Inventory>();

      if (playerInventory == null) return false;

      var flatList = new List<InventoryItem>();

      foreach (var shopItem in GetAllItems())
      {
        var inventoryItem = shopItem.InventoryItem;
        var quantity = shopItem.QuantityInTransaction; // 0 if not in transaction so it skips the loop

        for (int i = 0; i < quantity; i++)
        {
          flatList.Add(inventoryItem);
        }
      }

      return playerInventory.HasSpaceFor(flatList);
    }

    public bool HasSufficientFunds()
    {
      var playerBalance = _currentShopper.GetComponent<PlayerBalance>();

      if (playerBalance == null) return false;

      return playerBalance.CurrentBalance >= GetTransactionTotal();
    }

    public bool IsTransactionEmpty()
    {
      return _transaction.Count == 0;
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

      var availabilities = GetAvailabilities();
      var availability = availabilities[item];

      if (_transaction[item] + quantity > availability)
      {
        // we dont have enough stock
        // set the transaction item equal to the maximum we can have
        _transaction[item] = availability;
      }
      else
      {
        _transaction[item] += quantity;
      }

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
          if (_isBuyingMode)
          {
            PurchaseItem(shopperInventory, playerBalance, inventoryItem, price);
          }
          else
          {
            // selling mode
            SellItem(shopperInventory, playerBalance, inventoryItem, price);
          }
        }
      }

      // clear the transaction as safety
      _transaction.Clear();

      ONChange?.Invoke();
    }

    /// <summary>
    /// implements the selling logic
    /// items is first removed from the transaction dictionary foreach item
    /// </summary>
    private void SellItem(Inventory shopperInventory, PlayerBalance playerBalance, InventoryItem inventoryItem,
      float price)
    {
      var slot = FindFirstItemSlot(shopperInventory, inventoryItem);

      if (slot == -1) return;

      // remove from the transaction
      AddToTransaction(inventoryItem, -1);

      // remove the item from the player inventory
      shopperInventory.RemoveFromSlot(slot, 1);

      if (!_stockSold.ContainsKey(inventoryItem))
      {
        _stockSold[inventoryItem] = 0; // initialise this
      }

      // add the item to the shop stock
      _stockSold[inventoryItem]--;

      // pay the player
      playerBalance.UpdateBalance(price);
    }

    /// <summary>
    /// returns -1 if the item does not exist
    /// </summary>
    /// <param name="shopperInventory"></param>
    /// <param name="inventoryItem"></param>
    /// <returns></returns>
    private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem inventoryItem)
    {
      for (int i = 0; i < shopperInventory.GetSize(); i++)
      {
        if (inventoryItem == shopperInventory.GetItemInSlot(i))
        {
          return i;
        }
      }

      return -1;
    }

    private void PurchaseItem(Inventory shopperInventory, PlayerBalance playerBalance, InventoryItem inventoryItem,
      float price)
    {
      if (playerBalance.CurrentBalance < price)
      {
        return;
      }

      var success = shopperInventory.AddToFirstEmptySlot(inventoryItem, 1);

      if (success)
      {
        // remove from transaction
        AddToTransaction(inventoryItem, -1);

        if (!_stockSold.ContainsKey(inventoryItem))
        {
          _stockSold[inventoryItem] = 0; // initialise this
        }

        _stockSold[inventoryItem]++;

        // Debiting or crediting of funds
        playerBalance.UpdateBalance(-price);
      }
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

    /// <summary>
    /// gets the current shopper (player) level
    /// </summary>
    /// <returns></returns>
    private int GetShopperLevel()
    {
      var stats = _currentShopper.GetComponent<BaseStats>();

      return stats == null ? 0 : stats.GetPlayerLevel();
    }

    public object CaptureState()
    {
      var dictToSave = new Dictionary<string, int>();

      foreach (var pair in _stockSold)
      {
        dictToSave[pair.Key.ItemId] = pair.Value;
      }

      return dictToSave;
    }

    public void RestoreState(object state)
    {
      var saveObject = (Dictionary<string, int>) state;
      _stockSold.Clear();

      foreach (var pair in saveObject)
      {
        _stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
      }
    }
  }
}