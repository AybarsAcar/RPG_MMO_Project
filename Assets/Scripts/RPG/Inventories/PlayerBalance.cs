using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
  /// <summary>
  /// this class will keep track of Player's money
  /// attached to the Player prefab
  /// </summary>
  public class PlayerBalance : MonoBehaviour, IItemStore, ISavable
  {
    [SerializeField] private float startingBalance = 100f;

    private float _currentBalance = 0;
    public float CurrentBalance => _currentBalance;

    public event Action ONChange;

    private void Awake()
    {
      _currentBalance = startingBalance;
    }

    /// <summary>
    /// changes the balance
    /// </summary>
    /// <param name="amount">can be positive or negative</param>
    public void UpdateBalance(float amount)
    {
      _currentBalance += amount;

      // broadcast the balance change
      ONChange?.Invoke();
    }

    public int AddItems(InventoryItem item, int number)
    {
      // if not a currency don't accept it
      if (!(item is CurrencyItem)) return 0;

      UpdateBalance(item.Price * number);
      return number;
    }

    public object CaptureState()
    {
      return _currentBalance;
    }

    public void RestoreState(object state)
    {
      _currentBalance = (float) state;
    }
  }
}