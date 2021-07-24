using System;
using System.Collections.Generic;
using UnityEngine;

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
    public List<string> childIds = new List<string>();
    public Rect rect = new Rect(0, 0, 200, 100);
  }
}