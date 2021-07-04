using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories.StatInventories
{
  /// <summary>
  /// Wrapper class around Equipment
  /// which allows the wearable to improve player stats
  /// </summary>
  public class StatsEquipment : Equipment, IModifierProvider
  {
    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
      foreach (var slot in GetAllPopulatedSlots())
      {
        var item = GetItemInSlot(slot) as IModifierProvider; // make sure a Stat item
        if (item is null) continue;

        foreach (var modifier in item.GetAdditiveModifiers(stat))
        {
          yield return modifier;
        }
      }
    }

    public IEnumerable<float> GetPercentageModifier(Stat stat)
    {
      foreach (var slot in GetAllPopulatedSlots())
      {
        var item = GetItemInSlot(slot) as IModifierProvider; // make sure a Stat item
        if (item is null) continue;

        foreach (var modifier in item.GetPercentageModifier(stat))
        {
          yield return modifier;
        }
      }
    }
  }
}