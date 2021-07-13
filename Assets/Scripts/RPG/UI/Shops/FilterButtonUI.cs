using RPG.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
  /// <summary>
  /// Attached to each filter button in our UI
  /// </summary>
  public class FilterButtonUI : MonoBehaviour
  {
    [SerializeField] private ItemCategory category = ItemCategory.None;


    private Button _button;
    private Shop _currentShop;

    private void Awake()
    {
      _button = GetComponent<Button>();

      // register its callback
      _button.onClick.AddListener(SelectFilter);
    }

    public void SetCurrentShop(Shop currentShop)
    {
      _currentShop = currentShop;
    }

    private void SelectFilter()
    {
      // select the filter on the shop
      _currentShop.SelectFilter(category);
    }

    public void RefreshUI()
    {
      _button.interactable = _currentShop.Filter != category;
    }
  }
}