using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
  /// <summary>
  /// Node of the dialogue
  /// represents a piece of the dialogue
  /// Scriptable Object name property is used as its unique identifier
  /// </summary>
  public class DialogueNode : ScriptableObject
  {
    public string text;
    public List<string> childIds = new List<string>();
    public Rect rect = new Rect(0, 0, 200, 100);
  }
}