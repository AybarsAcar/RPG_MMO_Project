using System;
using UnityEngine;

namespace RPG.Inventories
{
  /// <summary>
  /// this class will keep track of Player's money
  /// attached to the Player prefab
  /// </summary>
  public class PlayerBalance : MonoBehaviour
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
  }
}