using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Dialogue
{
  /// <summary>
  /// Node of the dialogue
  /// represents a piece of the dialogue
  /// </summary>
  [Serializable]
  public class DialogueNode
  {
    public string id;
    public string text;
    public string[] childIds;
    public Rect rect = new Rect(0, 0, 200, 100);
  }
}