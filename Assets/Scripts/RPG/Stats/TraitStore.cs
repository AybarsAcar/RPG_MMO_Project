using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
  /// <summary>
  /// resides on the player
  /// </summary>
  public class TraitStore : MonoBehaviour, IModifierProvider, IPredicateEvaluator, ISavable
  {
    [Serializable]
    private struct TraitBonus
    {
      public Trait trait;
      public Stat stat;
      public float additiveBonusPerPoint;
      public float percentageBonusPerPoint;
    }

    [SerializeField] private TraitBonus[] bonusConfig;

    private Dictionary<Stat, Dictionary<Trait, float>> _additiveBonusCache;

    private Dictionary<Stat, Dictionary<Trait, float>> _percentageBonusCache;

    private Dictionary<Trait, int> _assignedPoints = new Dictionary<Trait, int>();

    // staged but not committed changes to our traits
    private readonly Dictionary<Trait, int> _stagedPoints = new Dictionary<Trait, int>();

    private void Awake()
    {
      _additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
      _percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();

      // build up the dictionaries
      foreach (var traitBonus in bonusConfig)
      {
        if (!_additiveBonusCache.ContainsKey(traitBonus.stat))
        {
          _additiveBonusCache[traitBonus.stat] = new Dictionary<Trait, float>();
        }

        if (!_percentageBonusCache.ContainsKey(traitBonus.stat))
        {
          _percentageBonusCache[traitBonus.stat] = new Dictionary<Trait, float>();
        }

        _additiveBonusCache[traitBonus.stat][traitBonus.trait] = traitBonus.additiveBonusPerPoint;
        _percentageBonusCache[traitBonus.stat][traitBonus.trait] = traitBonus.percentageBonusPerPoint;
      }
    }

    public int GetProposedPoints(Trait trait)
    {
      return GetPoints(trait) + GetStagedPoints(trait);
    }

    public int GetPoints(Trait trait)
    {
      return _assignedPoints.ContainsKey(trait) ? _assignedPoints[trait] : 0;
    }

    public int GetStagedPoints(Trait trait)
    {
      return _stagedPoints.ContainsKey(trait) ? _stagedPoints[trait] : 0;
    }

    public void AssignPoints(Trait trait, int points)
    {
      if (!CanAssignPoints(trait, points)) return;

      _stagedPoints[trait] = GetStagedPoints(trait) + points;
    }

    public bool CanAssignPoints(Trait trait, int points)
    {
      if (GetStagedPoints(trait) + points < 0) return false;
      if (GetUnassignedPoints() < points) return false;

      return true;
    }

    /// <summary>
    /// takes everything in the staged and puts into assigned dictionary
    /// then clears the stagedPoints dictionary
    /// </summary>
    public void Commit()
    {
      foreach (var trait in _stagedPoints.Keys)
      {
        _assignedPoints[trait] = GetProposedPoints(trait);
      }

      _stagedPoints.Clear();
    }

    public int GetUnassignedPoints()
    {
      return GetAssignablePoints() - GetTotalProposedPoints();
    }

    public int GetTotalProposedPoints()
    {
      return _assignedPoints.Values.Sum() + _stagedPoints.Values.Sum();
    }

    public int GetAssignablePoints()
    {
      return (int) GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
      if (!_additiveBonusCache.ContainsKey(stat)) yield break;

      foreach (var trait in _additiveBonusCache[stat].Keys)
      {
        var bonus = _additiveBonusCache[stat][trait];

        yield return bonus * GetPoints(trait);
      }
    }

    public IEnumerable<float> GetPercentageModifier(Stat stat)
    {
      if (!_percentageBonusCache.ContainsKey(stat)) yield break;

      foreach (var trait in _percentageBonusCache[stat].Keys)
      {
        var bonus = _percentageBonusCache[stat][trait];

        yield return bonus * GetPoints(trait);
      }
    }

    public bool? Evaluate(string predicate, string[] parameters)
    {
      if (predicate == "MinimumTrait")
      {
        if (Enum.TryParse<Trait>(parameters[0], out var trait))
        {
          return GetPoints(trait) >= int.Parse(parameters[1]);
        }
      }

      return null;
    }

    public object CaptureState()
    {
      return _assignedPoints;
    }

    public void RestoreState(object state)
    {
      _assignedPoints = new Dictionary<Trait, int>((IDictionary<Trait, int>) state);
    }
  }
}