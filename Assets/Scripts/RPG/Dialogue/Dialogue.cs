using UnityEngine;

namespace RPG.Dialogue
{
  [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
  public class Dialogue : ScriptableObject
  {
    [SerializeField] private DialogueNode[] nodes;
  }
}