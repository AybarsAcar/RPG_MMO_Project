using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Quests
{
  public class QuestStatus
  {
    private Quest _quest;
    public Quest GetQuest => _quest;

    private readonly List<string> _completedObjectives = new List<string>();

    public QuestStatus(Quest quest)
    {
      _quest = quest;
    }

    public int CompletedObjectiveCount => _completedObjectives.Count;

    public IEnumerable<string> GetCompletedObjectives()
    {
      return _completedObjectives;
    }

    public bool IsObjectiveComplete(string objective)
    {
      return _completedObjectives.Contains(objective);
    }
  }
}