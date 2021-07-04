using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Stats
{
  [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
  public class Progression : ScriptableObject
  {
    [SerializeField] private ProgressionCharacterClass[] characterClasses;

    [Serializable]
    private class ProgressionCharacterClass
    {
      public CharacterClass characterClass;

      // public float[] health;
      public ProgressionStat[] stats;
    }

    [Serializable]
    private class ProgressionStat
    {
      public Stat stat;
      public float[] levels;
    }

    private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

    public float GetStat(Stat stat, CharacterClass characterClass, int level)
    {
      BuildLookup();

      var levels = lookupTable[characterClass][stat];

      if (levels.Length < level) return 0f;

      return levels[level - 1];
    }

    public int GetLevels(Stat stat, CharacterClass characterClass)
    {
      BuildLookup();

      return lookupTable[characterClass][stat].Length;
    }

    private void BuildLookup()
    {
      if (lookupTable != null) return;

      lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

      foreach (var progressionCharacterClass in characterClasses)
      {
        var statLookupTable = new Dictionary<Stat, float[]>();

        foreach (var progressionStat in progressionCharacterClass.stats)
        {
          statLookupTable[progressionStat.stat] = progressionStat.levels;
        }

        lookupTable[progressionCharacterClass.characterClass] = statLookupTable;
      }
    }
  }
}