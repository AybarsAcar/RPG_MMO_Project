using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
  /// <summary>
  /// name of the file will be the name of the quest
  /// </summary>
  [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest", order = 0)]
  public class Quest : ScriptableObject
  {
    [SerializeField] private string[] objectives;
    
    public string Title => name;
    public int ObjectiveCount => objectives.Length;

    public IEnumerable<string> GetObjectives()
    {
      return objectives;
    }
  }
}