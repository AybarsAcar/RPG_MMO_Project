using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
  /// <summary>
  /// tracks the cooldown of abilities
  /// </summary>
  public class CooldownStore : MonoBehaviour
  {
    private readonly Dictionary<InventoryItem, float> _cooldownTimers = new Dictionary<InventoryItem, float>();
    private readonly Dictionary<InventoryItem, float> _initialCooldownTimes = new Dictionary<InventoryItem, float>();

    /// <summary>
    /// go through all the timers and decrement the Time.deltaTime
    /// </summary>
    private void Update()
    {
      // to avoid looping over the keys and mutating it at the same time
      var abilities = new List<InventoryItem>(_cooldownTimers.Keys);

      foreach (var ability in abilities)
      {
        _cooldownTimers[ability] -= Time.deltaTime;

        if (_cooldownTimers[ability] < 0)
        {
          _cooldownTimers.Remove(ability);
          _initialCooldownTimes.Remove(ability);
        }
      }
    }

    public void StartCooldown(InventoryItem ability, float cooldownTime)
    {
      _cooldownTimers[ability] = cooldownTime;
      _initialCooldownTimes[ability] = cooldownTime;
    }

    public float GetCooldownTimeRemaining(InventoryItem ability)
    {
      return _cooldownTimers.ContainsKey(ability) ? _cooldownTimers[ability] : 0f;
    }

    public float GetCooldownFractionRemaining(InventoryItem ability)
    {
      if (ability == null) return 0f;

      if (!_cooldownTimers.ContainsKey(ability)) return 0f;

      return _cooldownTimers[ability] / _initialCooldownTimes[ability];
    }
  }
}