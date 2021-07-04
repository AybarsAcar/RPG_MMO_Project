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
      _currentShop = _shopper.ActiveShop;

      gameObject.SetActive(_currentShop != null);

      if (_currentShop == null) return;

      shopName.text = _currentShop.ShopName;

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

        row.Setup(item);
      }
    }
  }
}