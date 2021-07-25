using RPG.Core.Util;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
  /// <summary>
  /// responsible for listing the Quest Scriptable objects in the UI
  /// attached to the Content of the Scroll View of the QuestUI
  /// </summary>
  public class QuestListUI : MonoBehaviour
  {
    [SerializeField] private QuestItemUI questPrefab;

    private QuestList _questList;

    private void Start()
    {
      _questList = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<QuestList>();

      _questList.OnUpdate += Redraw;
      
      Redraw();
    }

    private void Redraw()
    {
      // clear first
      foreach (GameObject child in transform)
      {
        Destroy(child);
      }

      foreach (var status in _questList.GetStatuses())
      {
        var uiInstance = Instantiate(questPrefab, transform);

        uiInstance.Setup(status);
      }
    }
  }
}