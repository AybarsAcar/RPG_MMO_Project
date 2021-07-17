using System;
using UnityEngine;

namespace RPG.Stats
{
  public class BaseStats : MonoBehaviour
  {
    [SerializeField] [Range(1, 100)] private int startingLevel = 1;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private Progression progression;

    [SerializeField] private GameObject levelUpFX = null;

    [SerializeField] private bool isUsingModifiers = false;

    public event Action OnLevelUp;

    private int _currentLevel = 0;

    private Experience _experience;

    private void Awake()
    {
      _experience = GetComponent<Experience>();
    }

    /// <summary>
    /// OnEnable is the standard place to register for callbacks
    /// </summary>
    private void OnEnable()
    {
      if (_experience != null)
      {
        _experience.OnExperienceGained += UpdateLevel; // subscribe to the delegate
      }
    }

    private void Start()
    {
      _currentLevel = CalculateLevel();
    }

    private void OnDisable()
    {
      if (_experience != null)
      {
        _experience.OnExperienceGained -= UpdateLevel; // remove subscription from the delegate
      }
    }

    private void UpdateLevel()
    {
      // update the current level
      var newLevel = CalculateLevel();

      if (newLevel > _currentLevel)
      {
        // implement level up
        _currentLevel = newLevel;

        // particle FX
        HandleLevelUpEffect();
        OnLevelUp();
      }
    }

    private void HandleLevelUpEffect()
    {
      Instantiate(levelUpFX, transform);
    }

    public float GetStat(Stat stat)
    {
      return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
    }

    private float GetPercentageModifier(Stat stat)
    {
      if (!isUsingModifiers)
      {
        return 0f;
      }

      var total = 0f;

      var providers = GetComponents<IModifierProvider>();
      foreach (var provider in providers)
      {
        foreach (var modifier in provider.GetPercentageModifier(stat))
        {
          total += modifier;
        }
      }

      return total;
    }

    private float GetBaseStat(Stat stat)
    {
      return progression.GetStat(stat, characterClass, GetPlayerLevel());
    }

    /// <summary>
    /// gets the modifiers and adds it to its total
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    private float GetAdditiveModifier(Stat stat)
    {
      if (!isUsingModifiers)
      {
        return 0f;
      }

      var total = 0f;

      var providers = GetComponents<IModifierProvider>();
      foreach (var provider in providers)
      {
        foreach (var modifier in provider.GetAdditiveModifiers(stat))
        {
          total += modifier;
        }
      }

      return total;
    }

    public int GetPlayerLevel()
    {
      if (_currentLevel < 1)
      {
        _currentLevel = CalculateLevel();
      }

      return _currentLevel;
    }


    private int CalculateLevel()
    {
      var experience = GetComponent<Experience>();

      if (experience == null) return startingLevel;

      var currentXp = experience.ExperiencePoints;

      var length = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

      for (int i = 1; i < length; i++)
      {
        var xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, i);

        if (xpToLevelUp > currentXp)
        {
          return i;
        }
      }

      return length;
    }
  }
}