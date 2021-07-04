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
  /// on the shop keeper or hte object that requires to behave like the shop
  /// this class also determines which items a shop will have
  /// </summary>
  public class Shop : MonoBehaviour, IRaycastable
  {
    [Serializable]
    private struct StockConfig
    {
      public InventoryItem Item;
      public int InitialStock;
      [Range(0, 100)] public float BuyingDiscountPercentage;
    }

    [SerializeField] private StockConfig[] stockConfigs;

    [SerializeField] private string shopName;
    public string ShopName => shopName;

    public event Action ONChange;

    public IEnumerable<ShopItem> GetFilteredItems()
    {
      return stockConfigs.Select(config => new ShopItem
      {
        Item = config.Item,
        Availability = config.InitialStock,
        Price = config.Item.Price * (1 - (config.BuyingDiscountPercentage / 100)),
        QuantityInTransaction = 0
      });
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
      return 0f;
    }

    public void AddToTransaction(InventoryItem item, int quantity)
    {
    }

    /// <summary>
    /// confirms selling an buying transactions
    /// </summary>
    public void ConfirmTransaction()
    {
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