using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
  public class DialogueEditor : EditorWindow
  {
    private Dialogue _selectedDialogue;

    private Vector2 _scrollPosition;

    [NonSerialized] private GUIStyle _nodeStyle;

    // parent node that recently created dialogue node as its child
    [NonSerialized] private DialogueNode _creatingNode;

    // node that is currently being deleted
    [NonSerialized] private DialogueNode _deletingNode;

    [NonSerialized] private DialogueNode _linkingParentNode;

    // currently dragged node
    [NonSerialized] private DialogueNode _draggingNode;
    [NonSerialized] private Vector2 _draggingOffset;

    // dragging the canvas by clicking and dragging
    [NonSerialized] private bool _isDraggingCanvas;
    [NonSerialized] private Vector2 _draggingCanvasOffset;

    private const float CanvasWidth = 4000f;
    private const float CanvasHeight = 2000f;
    private const float BackgroundSize = 50f;

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

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        var canvas = GUILayoutUtility.GetRect(CanvasWidth, CanvasHeight);

        // load the background
        var background = (Texture2D) Resources.Load("background");

        // textCoord is how many times the texture requires to repeat
        var textCoord = new Rect(0, 0, CanvasWidth / BackgroundSize, CanvasHeight / BackgroundSize);
        GUI.DrawTextureWithTexCoords(canvas, background, textCoord);

        foreach (var dialogueNode in _selectedDialogue.GetAllNodes())
        {
          DrawConnections(dialogueNode);
        }

        foreach (var dialogueNode in _selectedDialogue.GetAllNodes())
        {
          DrawNode(dialogueNode);
        }

        EditorGUILayout.EndScrollView();

        // add the node after the layout is drawn is a new node is created
        // this change will rerender the Editor
        if (_creatingNode != null)
        {
          // create the child node
          Undo.RecordObject(_selectedDialogue, "Added Dialogue Node");

          _selectedDialogue.CreateNode(_creatingNode);
          _creatingNode = null;
        }

        // delete the node if there is a deleting node
        if (_deletingNode != null)
        {
          Undo.RecordObject(_selectedDialogue, "Added Dialogue Node");

          _selectedDialogue.DeleteNode(_deletingNode);
          _deletingNode = null;
        }
      }
    }

    /// <summary>
    /// Draws the bezier curve from the parent to the children
    /// </summary>
    /// <param name="dialogueNode"></param>
    private void DrawConnections(DialogueNode dialogueNode)
    {
      var startPos = new Vector3(dialogueNode.rect.xMax, dialogueNode.rect.center.y);

      foreach (var child in _selectedDialogue.GetAllChildren(dialogueNode))
      {
        var endPos = new Vector3(child.rect.xMin, child.rect.center.y);
        var controlPointOffset = endPos - startPos;
        controlPointOffset.y = 0;
        controlPointOffset.x *= 0.8f;

        Handles.DrawBezier(startPos, endPos, startPos + controlPointOffset, endPos - controlPointOffset, Color.white,
          Texture2D.whiteTexture, 2f);
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
        // pass in the scroll offset to get take into scroll position into consideration
        // otherwise it won't select the node
        _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);

        if (_draggingNode != null)
        {
          _draggingOffset = _draggingNode.rect.position - Event.current.mousePosition;
        }
        else
        {
          // dragging node is null - an empty space is clicked
          // record the drag offset and dragging
          _isDraggingCanvas = true;
          _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
        }
      }
      else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
      {
        Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");

        // update the position
        _draggingNode.rect.position = Event.current.mousePosition + _draggingOffset;

        GUI.changed = true; // so it repaints
      }
      else if (Event.current.type == EventType.MouseDrag && _isDraggingCanvas)
      {
        // update the scroll position
        _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;

        // repaint
        GUI.changed = true;
      }
      else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
      {
        _draggingNode = null;
      }
      else if (Event.current.type == EventType.MouseUp && _isDraggingCanvas)
      {
        _isDraggingCanvas = false;
      }
    }

    /// <summary>
    /// Draws the node on the Window
    /// </summary>
    /// <param name="dialogueNode"></param>
    private void DrawNode(DialogueNode dialogueNode)
    {
      GUILayout.BeginArea(dialogueNode.rect, _nodeStyle);

      EditorGUI.BeginChangeCheck();

      var newDialogueText = EditorGUILayout.TextField(dialogueNode.text);

      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(_selectedDialogue, "Update Dialogue Text");

        // update the scriptable object
        dialogueNode.text = newDialogueText;
      }

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("x"))
      {
        _deletingNode = dialogueNode;
      }

      // add a GUI Layout button to add a child node
      if (GUILayout.Button("+"))
      {
        _creatingNode = dialogueNode;
      }

      GUILayout.EndHorizontal();

      // Conditionally renders the link buttons
      // and handles the linking process
      DrawLinkButtons(dialogueNode);

      GUILayout.EndArea();
    }

    private void DrawLinkButtons(DialogueNode currentNode)
    {
      if (_linkingParentNode == null)
      {
        // set to the linking mode by setting the current node as the linking node
        if (GUILayout.Button("Link"))
        {
          // set it to the current node
          _linkingParentNode = currentNode;
        }
      }
      else if (_linkingParentNode == currentNode)
      {
        //the node is the current button
        if (GUILayout.Button("Cancel"))
        {
          _linkingParentNode = null;
        }
      }
      else if (_linkingParentNode.childIds.Contains(currentNode.id))
      {
        // the node is already a child 
        if (GUILayout.Button("Unlink"))
        {
          // cancel the link
          _linkingParentNode.childIds.Remove(currentNode.id);
        }
      }
      else
      {
        if (GUILayout.Button("Child"))
        {
          Undo.RecordObject(_selectedDialogue, "Add Dialogue Link");

          // add the child
          _linkingParentNode.childIds.Add(currentNode.id);
        }
      }
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