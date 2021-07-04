using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
  public class RowUI : MonoBehaviour
  {
    [Header("Item Fields")]
    [SerializeField] private Image iconField;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI availabilityField;
    [SerializeField] private TextMeshProUGUI priceField;
    [SerializeField] private TextMeshProUGUI quantityField;
    
    /// <summary>
    /// sets up the display information based on the data passed in
    /// </summary>
    /// <param name="item">Shop Item data</param>
    public void Setup(ShopItem item)
    {
      iconField.sprite = item.Item.Icon;
      nameField.text = item.Item.DisplayName;
      availabilityField.text = item.Availability.ToString();
      priceField.text = $"$ {item.Price:n}";
      quantityField.text = $"- {item.QuantityInTransaction} +";
    }
  }
}