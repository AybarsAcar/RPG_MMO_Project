using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
  /// <summary>
  /// attached to the Player object
  /// </summary>
  public class QuestList : MonoBehaviour
  {
    private List<QuestStatus> _statuses = new List<QuestStatus>();

    public event Action OnUpdate;
    
    public IEnumerable<QuestStatus> GetStatuses()
    {
      return _statuses;
    }

    public void AddQuest(Quest quest)
    {
      if (HasQuest(quest)) return;

      var status = new QuestStatus(quest);
      _statuses.Add(status);
      
      OnUpdate?.Invoke();
    }

    /// <summary>
    /// used to prevent adding a quest more than once to the quest list
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public bool HasQuest(Quest quest)
    {
      return _statuses.Any(status => status.GetQuest == quest);
    }
  }
}