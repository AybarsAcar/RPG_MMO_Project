using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
  public class QuestTooltipUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Transform objectiveContainer;

    [SerializeField] private GameObject objectivePrefab;
    [SerializeField] private GameObject objectiveIncompletePrefab;

    [SerializeField] private TextMeshProUGUI rewardText;

    /// <summary>
    /// called in the quest tooltip spawner
    /// </summary>
    /// <param name="status"></param>
    public void Setup(QuestStatus status)
    {
      var quest = status.GetQuest;

      title.text = quest.Title;

      // clear objectives
      foreach (GameObject child in objectiveContainer)
      {
        Destroy(child);
      }

      foreach (var objective in quest.GetObjectives())
      {
        var prefab = status.IsObjectiveComplete(objective.reference) ? objectivePrefab : objectiveIncompletePrefab;

        var objectiveInstance = Instantiate(prefab, objectiveContainer);

        var tmp = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();

        tmp.text = objective.description;
      }

      rewardText.text = GetRewardText(quest);
    }

    private string GetRewardText(Quest quest)
    {
      var sb = string.Empty;

      foreach (var reward in quest.GetRewards())
      {
        if (sb != string.Empty)
        {
          sb += ", ";
        }

        if (reward.number > 1)
        {
          sb += $"{reward.number} ";
        }

        sb += reward.item.DisplayName;
      }

      if (sb == string.Empty)
      {
        sb = "No reward";
      }

      sb += ".";
      return sb;
    }
  }
}