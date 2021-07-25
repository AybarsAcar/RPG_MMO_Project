using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
  /// <summary>
  /// responsible for displaying the Quest Item
  /// attached to the Quest Prefab which is a UI element
  /// </summary>
  public class QuestItemUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI progress;

    private QuestStatus _status;
    public QuestStatus Status => _status;
    
    public void Setup(QuestStatus status)
    {
      _status = status;
      title.text = status.GetQuest.Title;
      progress.text = $"{status.CompletedObjectiveCount}/{status.GetQuest.ObjectiveCount}";
    }
  }
}