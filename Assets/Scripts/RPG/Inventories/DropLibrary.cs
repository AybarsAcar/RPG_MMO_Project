using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Inventories
{
  /// <summary>
  /// Centralised location for our enemy drops
  /// we implement a relative chance
  /// i.e it is more common to get a hat than the fireball etc
  /// </summary>
  [CreateAssetMenu(fileName = "Drop Library", menuName = "InventorySystem/New Drop Library", order = 0)]
  public class DropLibrary : ScriptableObject
  {
    [Serializable]
    private class DropConfig
    {
      public InventoryItem item;
      public float[] relativeChance;
      public int[] minNumber;
      public int[] maxNumber;

      public int GetRandomNumber(int level)
      {
        if (!item.IsStackable) return 1;

        var min = GetByLevel(minNumber, level);
        var max = GetByLevel(maxNumber, level);
        return Random.Range(min, max + 1);
      }
    }

    [SerializeField] private DropConfig[] potentialDrops;

    [SerializeField] private float[] dropChancePercentage;
    [SerializeField] private int[] minDrops;
    [SerializeField] private int[] maxDrops;

    public struct Dropped
    {
      public InventoryItem item;
      public int number;
    }

    public IEnumerable<Dropped> GetRandomDrops(int level)
    {
      if (!ShouldRandomDrop(level)) yield break;

      for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
      {
        yield return GetRandomDrop(level);
      }
    }

    private Dropped GetRandomDrop(int level)
    {
      var drop = SelectRandomItem(level);
      
      return new Dropped
      {
        item = drop.item,
        number = drop.GetRandomNumber(level)
      };
    }

    private DropConfig SelectRandomItem(int level)
    {
      var randomRoll = Random.Range(0, GetTotalChance(level));
      var chanceTotal = 0f;
      foreach (var drop in potentialDrops)
      {
        chanceTotal += GetByLevel(drop.relativeChance, level);
        if (chanceTotal > randomRoll)
        {
          return drop;
        }
      }

      return null;
    }

    private float GetTotalChance(int level)
    {
      return potentialDrops.Sum(drop => GetByLevel(drop.relativeChance, level));
    }


    private int GetRandomNumberOfDrops(int level)
    {
      var min = GetByLevel(minDrops, level);
      var max = GetByLevel(maxDrops, level);

      return Random.Range(min, max);
    }

    private bool ShouldRandomDrop(int level)
    {
      return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
    }

    /// <summary>
    /// Generic method
    /// </summary>
    /// <param name="values"></param>
    /// <param name="level"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T GetByLevel<T>(T[] values, int level)
    {
      if (values.Length == 0)
      {
        return default;
      }

      if (level > values.Length)
      {
        return values[values.Length - 1];
      }

      if (level <= 0)
      {
        return default;
      }

      return values[level - 1];
    }
  }
}