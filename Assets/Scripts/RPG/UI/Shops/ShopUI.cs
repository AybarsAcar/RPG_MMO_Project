using RPG.Core.Util;
using RPG.Shops;
using TMPro;
using UnityEngine;

namespace RPG.UI.Shops
{
  public class ShopUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI shopName;
    [SerializeField] private Transform shoppingListRoot;
    [SerializeField] private RowUI rowPrefab;
    [SerializeField] private TextMeshProUGUI totalField;

    private Shopper _shopper;
    private Shop _currentShop;

    private void Start()
    {
      _shopper = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<Shopper>();

      if (_shopper == null) return;

      _shopper.activeShopChanged += HandleShopChanged;

      HandleShopChanged();
    }

    public void Close()
    {
      _shopper.SetActiveShop(null);
    }

    private void HandleShopChanged()
    {
      // remove subscription from the old shop
      if (_currentShop != null)
      {
        _currentShop.ONChange -= RefreshUI;
      }
      
      _currentShop = _shopper.ActiveShop;

      gameObject.SetActive(_currentShop != null);

      if (_currentShop == null) return;

      shopName.text = _currentShop.ShopName;

      // subscribe to RefreshUI method
      _currentShop.ONChange += RefreshUI;

      RefreshUI();
    }

    /// <summary>
    /// Builds a UI List
    /// </summary>
    private void RefreshUI()
    {
      // delete each of the items in there
      foreach (Transform child in shoppingListRoot)
      {
        Destroy(child.gameObject);
      }

      // refill the items
      foreach (var item in _currentShop.GetFilteredItems())
      {
        var row  =Instantiate<RowUI>(rowPrefab, parent: shoppingListRoot);

        row.Setup(_currentShop, item);
      }
      
      // update the Total Cost field
      totalField.text = $"Total: ${_currentShop.GetTransactionTotal():n}";
    }

    /// <summary>
    /// called from the buy / sell button
    /// </summary>
    public void ConfirmTransaction()
    {
      _currentShop.ConfirmTransaction();
    }
  }
}