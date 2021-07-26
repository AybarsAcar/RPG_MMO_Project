using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Inventories;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
  /// <summary>
  /// attached to the Player object
  /// </summary>
  public class QuestList : MonoBehaviour, ISavable
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

    public void CompleteObjective(Quest quest, string objective)
    {
      foreach (var status in _statuses)
      {
        if (status.GetQuest != quest) continue;

        status.CompleteObjective(objective);

        if (status.IsComplete())
        {
          GiveReward(quest);
        }

        OnUpdate?.Invoke();
      }
    }

    private void GiveReward(Quest quest)
    {
      foreach (var reward in quest.GetRewards())
      {
        var success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);

        if (!success)
        {
          // if there's not enough space in the inventory, it drops
          // so it will create a pickup near the player
          GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
        }
      }
    }

    public object CaptureState()
    {
      var state = new List<object>();
      foreach (var status in _statuses)
      {
        state.Add(status.CaptureState());
      }

      return state;
    }

    public void RestoreState(object state)
    {
      var stateList = state as List<object>;

      if (state == null) return;

      _statuses.Clear();

      foreach (object objectState in stateList)
      {
        _statuses.Add(new QuestStatus(objectState));
      }
    }
  }
}