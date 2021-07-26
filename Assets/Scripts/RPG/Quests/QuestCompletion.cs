using RPG.Core.Util;
using UnityEngine;

namespace RPG.Quests
{
  /// <summary>
  /// 
  /// </summary>
  public class QuestCompletion : MonoBehaviour
  {
    [SerializeField] private Quest quest;
    [SerializeField] private string objective;

    public void CompleteObjective()
    {
      // get hold of the questList from the Player
      var questList = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<QuestList>();

      questList.CompleteObjective(quest, objective);
    }
  }
}