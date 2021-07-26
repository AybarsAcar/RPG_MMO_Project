using System;
using System.Collections.Generic;

namespace RPG.Quests
{
  public class QuestStatus
  {
    [Serializable]
    private class QuestStatusRecord
    {
      public string questName;
      public List<string> completedObjectives;
    }

    private Quest _quest;
    public Quest GetQuest => _quest;

    private readonly List<string> _completedObjectives = new List<string>();

    public QuestStatus(object objectState)
    {
      var state = objectState as QuestStatusRecord;

      _quest = Quest.GetByName(state.questName);
      _completedObjectives = state.completedObjectives;
    }

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

    public void CompleteObjective(string objective)
    {
      if (_quest.HasObjective(objective))
      {
        _completedObjectives.Add(objective);
      }
    }

    public object CaptureState()
    {
      return new QuestStatusRecord
      {
        questName = _quest.name,
        completedObjectives = _completedObjectives
      };
    }

    public bool IsComplete()
    {
      return _completedObjectives.Count == _quest.ObjectiveCount;
    }
  }
}