using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories.StatInventories
{
  /// <summary>
  /// wrapper class for the Equipable item that gives bonus stats to the player
  /// implemented to have a similar design pattern with the weapons
  /// </summary>
  [CreateAssetMenu(fileName = "New Stats Equipable Item", menuName = "Stats Inventory/New Stats Equipable Item",
    order = 0)]
  public class StatsEquipableItem : EquipableItem, IModifierProvider
  {
    [SerializeField] private Modifier[] flatModifiers; // additive flat bonuses
    [SerializeField] private Modifier[] percentModifier; // percentage multiplier bonuses


    [Serializable]
    private struct Modifier
    {
      public Stat stat;
      public float value;
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
      foreach (var modifier in flatModifiers)
      {
        if (modifier.stat == stat)
        {
          yield return modifier.value;
        }
      }
    }

    public IEnumerable<float> GetPercentageModifier(Stat stat)
    {
      foreach (var modifier in percentModifier)
      {
        if (modifier.stat == stat)
        {
          yield return modifier.value;
        }
      }
    }
  }
}