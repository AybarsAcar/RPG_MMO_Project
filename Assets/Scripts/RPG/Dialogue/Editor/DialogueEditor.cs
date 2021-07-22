using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
  public class DialogueEditor : EditorWindow
  {
    private Dialogue _selectedDialogue;

    private GUIStyle _nodeStyle;

    // currently dragged node
    private DialogueNode _draggingNode;
    private Vector2 _draggingOffset;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowEditorWindow()
    {
      GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
    }

    /// <summary>
    /// called when the Dialogue Asset is clicked
    /// it automatically launches the DialogueEditor if not already open
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceId, int line)
    {
      // grab if the clicked object is a dialogue
      var dialogue = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;

      if (dialogue == null) return false;

      ShowEditorWindow();
      return true;
    }

    private void OnEnable()
    {
      Selection.selectionChanged += OnSelectionChanged;

      _nodeStyle = new GUIStyle
      {
        normal =
        {
          background = EditorGUIUtility.Load("node0") as Texture2D,
        },
        padding = new RectOffset(20, 20, 20, 20),
        border = new RectOffset(12, 12, 12, 12)
      };
    }

    private void OnSelectionChanged()
    {
      var dialogue = (Dialogue) Selection.activeObject;

      if (dialogue == null) return;

      _selectedDialogue = dialogue;
      Repaint();
    }

    private void OnGUI()
    {
      if (_selectedDialogue == null)
      {
        EditorGUILayout.LabelField("No Dialogue selected");
      }
      else
      {
        ProcessEvents();

        foreach (var dialogueNode in _selectedDialogue.GetAllNodes())
        {
          OnGUINode(dialogueNode);
        }
      }
    }

    /// <summary>
    /// handles the selecting the dialogue on click
    /// and dragging events
    /// </summary>
    private void ProcessEvents()
    {
      if (Event.current.type == EventType.MouseDown && _draggingNode == null)
      {
        _draggingNode = GetNodeAtPoint(Event.current.mousePosition);

        if (_draggingNode != null)
        {
          _draggingOffset = _draggingNode.rect.position - Event.current.mousePosition;
        }
      }
      else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
      {
        Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");

        // update the position
        _draggingNode.rect.position = Event.current.mousePosition + _draggingOffset;

        GUI.changed = true; // so it repaints
      }
      else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
      {
        _draggingNode = null;
      }
    }

    private void OnGUINode(DialogueNode dialogueNode)
    {
      GUILayout.BeginArea(dialogueNode.rect, _nodeStyle);

      EditorGUI.BeginChangeCheck();

      EditorGUILayout.LabelField("Node:", EditorStyles.boldLabel);
      var newDialogueId = EditorGUILayout.TextField(dialogueNode.id);
      var newDialogueText = EditorGUILayout.TextField(dialogueNode.text);

      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(_selectedDialogue, "Update Dialogue Text");

        // update the scriptable object
        dialogueNode.text = newDialogueText;
        dialogueNode.id = newDialogueId;
      }

      GUILayout.EndArea();
    }


    /// <summary>
    /// gets the node which is selected
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private DialogueNode GetNodeAtPoint(Vector2 point)
    {
      DialogueNode foundNode = null;
      foreach (var dialogueNode in _selectedDialogue.GetAllNodes())
      {
        if (dialogueNode.rect.Contains(point))
        {
          foundNode = dialogueNode;
        }
      }

      return foundNode;
    }
  }
}