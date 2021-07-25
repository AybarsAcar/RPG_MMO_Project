using RPG.Core.Util;
using UnityEngine;

namespace RPG.Quests
{
  /// <summary>
  /// attached to the AI game objects that can give quests to the player
  /// </summary>
  public class QuestGiver : MonoBehaviour
  {
    [SerializeField] private Quest quest;

    /// <summary>
    /// triggered on the current game object Dialogue Trigger
    /// </summary>
    public void GiveQuest()
    {
      var questList = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<QuestList>();
      questList.AddQuest(quest);
    }
  }
}