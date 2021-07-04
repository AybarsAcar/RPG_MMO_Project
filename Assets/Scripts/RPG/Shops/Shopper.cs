using System;
using UnityEngine;

namespace RPG.Shops
{
  /// <summary>
  /// dictates what UI can display
  /// it resides on the main player object
  /// </summary>
  public class Shopper : MonoBehaviour
  {
    private Shop _activeShop;
    public Shop ActiveShop => _activeShop;

    public event Action activeShopChanged;
    
    public void SetActiveShop(Shop shop)
    {
      _activeShop = shop;

      if (activeShopChanged != null)
      {
        activeShopChanged.Invoke();
      }
    }
  }
}