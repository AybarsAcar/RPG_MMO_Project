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

    /// <summary>
    /// called in the quest tooltip spawner
    /// </summary>
    /// <param name="quest"></param>
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
        var prefab = status.IsObjectiveComplete(objective) ? objectivePrefab : objectiveIncompletePrefab;
        
        var objectiveInstance = Instantiate(prefab, objectiveContainer);
        
        var tmp = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();

        tmp.text = objective;
      }
    }
  }
}