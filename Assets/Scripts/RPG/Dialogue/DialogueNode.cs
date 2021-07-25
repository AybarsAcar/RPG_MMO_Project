using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] private bool isPlayerSpeaking;
    public bool IsPlayerSpeaking => isPlayerSpeaking;

    [SerializeField] private string text;
    public string Text => text;

    [SerializeField] private List<string> childIds = new List<string>();
    public List<string> ChildIds => childIds;

    [SerializeField] private Rect rect = new Rect(0, 0, 200, 100);
    public Rect Rect => rect;

#if UNITY_EDITOR

    public void SetPosition(Vector2 newPosition)
    {
      Undo.RecordObject(this, "Move Dialogue Node");

      rect.position = newPosition;

      EditorUtility.SetDirty(this);
    }

    public void SetText(string newDialogueText)
    {
      if (newDialogueText == text) return;

      Undo.RecordObject(this, "Update Dialogue Text");
      text = newDialogueText;

      EditorUtility.SetDirty(this);
    }

    public void AddChild(string childId)
    {
      Undo.RecordObject(this, "Add Dialogue Link");

      childIds.Add(childId);

      EditorUtility.SetDirty(this);
    }

    public void RemoveChild(string childId)
    {
      Undo.RecordObject(this, "Remove Dialogue Link");

      childIds.Remove(childId);

      EditorUtility.SetDirty(this);
    }
#endif
    public void SetIsPlayerSpeaking(bool b)
    {
      Undo.RecordObject(this, "Change Dialogue Speaker");

      isPlayerSpeaking = b;

      EditorUtility.SetDirty(this);
    }
  }
}