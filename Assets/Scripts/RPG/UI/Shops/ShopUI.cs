using RPG.Core.Util;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
  public class ShopUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI shopName;
    [SerializeField] private Transform shoppingListRoot;
    [SerializeField] private RowUI rowPrefab;
    [SerializeField] private TextMeshProUGUI totalField;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button toggleBuyingModeButton;

    private Shopper _shopper;
    private Shop _currentShop;

    private Color _defaultTotalTextColor;

    private void Start()
    {
      _defaultTotalTextColor = totalField.color;

      _shopper = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<Shopper>();

      if (_shopper == null) return;

      _shopper.activeShopChanged += HandleShopChanged;

      // link up the onClick callback function to the button
      confirmButton.onClick.AddListener(ConfirmTransaction);
      toggleBuyingModeButton.onClick.AddListener(SwitchMode);

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

      foreach (var filterButtonUI in GetComponentsInChildren<FilterButtonUI>())
      {
        filterButtonUI.SetCurrentShop(_currentShop);
      }

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
        var row = Instantiate<RowUI>(rowPrefab, parent: shoppingListRoot);

        row.Setup(_currentShop, item);
      }

      // update the Total Cost field
      totalField.text = $"Total: ${_currentShop.GetTransactionTotal():n}";

      totalField.color = _currentShop.HasSufficientFunds() ? _defaultTotalTextColor : Color.red;

      confirmButton.interactable = _currentShop.CanTransact();

      var toggleButtonText = toggleBuyingModeButton.GetComponentInChildren<TextMeshProUGUI>();
      var confirmButtonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();

      if (_currentShop.IsBuyingMode)
      {
        // set the text of the toggle button
        toggleButtonText.text = "Switch to Selling";
        confirmButtonText.text = "Buy";
      }
      else
      {
        toggleButtonText.text = "Switch to Buying";
        confirmButtonText.text = "Sell";
      }

      // update the filter button state as well
      foreach (var filterButtonUI in GetComponentsInChildren<FilterButtonUI>())
      {
        filterButtonUI.RefreshUI();
      }
    }

    /// <summary>
    /// called from the buy / sell button
    /// </summary>
    public void ConfirmTransaction()
    {
      _currentShop.ConfirmTransaction();
    }

    public void SwitchMode()
    {
      _currentShop.SelectMode(!_currentShop.IsBuyingMode);
    }
  }
}