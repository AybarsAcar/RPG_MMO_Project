using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
  public class RowUI : MonoBehaviour
  {
    [Header("Item Fields")] [SerializeField]
    private Image iconField;

    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI availabilityField;
    [SerializeField] private TextMeshProUGUI priceField;
    [SerializeField] private TextMeshProUGUI quantityField;

    private Shop _currentShop;
    private ShopItem _item;

    /// <summary>
    /// sets up the display information based on the data passed in
    /// </summary>
    /// <param name="shop">Current shop we have clicked passed in from ShopUI</param>
    /// <param name="item">Shop Item data</param>
    public void Setup(Shop shop, ShopItem item)
    {
      iconField.sprite = item.InventoryItem.Icon;
      nameField.text = item.InventoryItem.DisplayName;
      availabilityField.text = item.Availability.ToString();
      priceField.text = $"$ {item.Price:n}";
      quantityField.text = item.QuantityInTransaction.ToString();

      _currentShop = shop;
      _item = item;
    }

    /// <summary>
    /// the following method will be called from the buttons
    /// onClick events
    /// </summary>
    public void Add()
    {
      _currentShop.AddToTransaction(_item.InventoryItem, 1);
    }

    public void Remove()
    {
      _currentShop.AddToTransaction(_item.InventoryItem, -1);
    }
  }
}