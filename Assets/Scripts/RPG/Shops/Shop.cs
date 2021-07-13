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
      public InventoryItem InventoryItem; // our Scriptable object
      public int InitialStock;
      [Range(0, 100)] public float BuyingDiscountPercentage;
    }

    // so the player will be able to sell the item for the 60% of its original price
    [Tooltip("The value is a fraction")] [SerializeField] [Range(0, 1)]
    private float sellingRate = 0.6f;

    [SerializeField] private StockConfig[] stockConfigs;

    [SerializeField] private string shopName;
    public string ShopName => shopName;

    private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();

    // keeps track of the shop stock
    private Dictionary<InventoryItem, int> _stock = new Dictionary<InventoryItem, int>();
    private Shopper _currentShopper;

    private bool _isBuyingMode = true;
    public bool IsBuyingMode => _isBuyingMode;

    private ItemCategory filter = ItemCategory.None;
    public ItemCategory Filter => filter;

    public event Action ONChange;

    private void Awake()
    {
      // initialise the stock dictionary
      foreach (var stockConfig in stockConfigs)
      {
        _stock[stockConfig.InventoryItem] = stockConfig.InitialStock;
      }
    }

    public void SetShopper(Shopper shopper)
    {
      _currentShopper = shopper;
    }

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
      foreach (var config in stockConfigs)
      {
        _transaction.TryGetValue(config.InventoryItem, out var quantityInTransaction);

        yield return new ShopItem
        {
          InventoryItem = config.InventoryItem,
          Availability = CalculateAvailability(config.InventoryItem),
          Price = CalculatePrice(config),
          QuantityInTransaction = quantityInTransaction,
        };
      }
    }

    /// <summary>
    /// returns the stock availability in the shop if in buying mode
    /// returns the stock in player inventory if in selling mode
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    private int CalculateAvailability(InventoryItem inventoryItem)
    {
      if (_isBuyingMode)
      {
        return _stock[inventoryItem];
      }

      return CountItemsInInventory(inventoryItem);
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

    private float CalculatePrice(StockConfig config)
    {
      if (_isBuyingMode)
      {
        return config.InventoryItem.Price * (1 - (config.BuyingDiscountPercentage / 100));
      }

      return config.InventoryItem.Price * sellingRate;
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

      var availability = CalculateAvailability(item);
      print(availability);
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

      // add the item to the shop stock
      _stock[inventoryItem]++;

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

        _stock[inventoryItem]--;

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
  }
}