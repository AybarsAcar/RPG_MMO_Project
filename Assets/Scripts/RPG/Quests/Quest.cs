using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Quests
{
  /// <summary>
  /// name of the file will be the name of the quest
  /// </summary>
  [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest", order = 0)]
  public class Quest : ScriptableObject
  {
    [SerializeField] private List<Objective> objectives = new List<Objective>();
    [SerializeField] private List<Reward> rewards = new List<Reward>();

    /// <summary>
    /// the reward of finishing the quest
    /// </summary>
    [Serializable]
    public class Reward
    {
      [Min(1)] public int number; // number of items to reward
      public InventoryItem item; // the item to reward
    }

    [Serializable]
    public class Objective
    {
      public string reference;
      public string description;

      public Condition completionCondition;
      public bool usesCondition = false;
    }

    public string Title => name;
    public int ObjectiveCount => objectives.Count;


    /// <summary>
    /// returns the quest object based on it's file name
    /// </summary>
    /// <param name="questName"></param>
    /// <returns></returns>
    public static Quest GetByName(string questName)
    {
      foreach (var quest in Resources.LoadAll<Quest>(""))
      {
        if (quest.name == questName)
        {
          return quest;
        }
      }

      return null;
    }


    public IEnumerable<Objective> GetObjectives()
    {
      return objectives;
    }

    public IEnumerable<Reward> GetRewards()
    {
      return rewards;
    }

    public bool HasObjective(string objectiveReference)
    {
      return objectives.Any(o => o.reference == objectiveReference);
    }
  }
}