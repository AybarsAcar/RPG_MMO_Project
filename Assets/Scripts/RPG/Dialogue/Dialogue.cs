using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
  [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
  public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
  {
    [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

    [SerializeField] private Vector2 newNodeOffset = new Vector2(250f, 0);

    private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
    private void Awake()
    {
      // so its called when built as well
      OnValidate();
    }
#endif

    /// <summary>
    /// called when the values is changed or the script is loaded
    /// </summary>
    private void OnValidate()
    {
      if (nodes.Count == 0)
      {
        CreateNode(null);
      }

      _nodeLookup.Clear();

      foreach (var dialogueNode in nodes)
      {
        _nodeLookup[dialogueNode.name] = dialogueNode;
      }
    }

    public IEnumerable<DialogueNode> GetAllNodes()
    {
      return nodes;
    }

    public DialogueNode GetRootNode()
    {
      return nodes[0];
    }

    /// <summary>
    /// gets all the children
    /// </summary>
    /// <param name="parentNode"></param>
    /// <returns></returns>
    public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
    {
      foreach (var childId in parentNode.ChildIds)
      {
        if (_nodeLookup.ContainsKey(childId))
        {
          yield return _nodeLookup[childId];
        }
      }
    }


#if UNITY_EDITOR
    /// <summary>
    /// Creates a Dialogue node as a child
    /// creates a child node if parent is passed, if not creates the initial root node
    /// </summary>
    /// <param name="parent">Parent node of the newly created node</param>
    public void CreateNode(DialogueNode parent = null)
    {
      var child = CreateInstance<DialogueNode>();
      child.name = Guid.NewGuid().ToString();

      Undo.RegisterCreatedObjectUndo(child, "CreatedDialogueNode");

      if (parent != null)
      {
        parent.AddChild(child.name);
        child.SetIsPlayerSpeaking(!parent.IsPlayerSpeaking);
        child.SetPosition(parent.Rect.position + newNodeOffset);
      }

      Undo.RecordObject(this, "Added Dialogue Node");

      // added to the global nodes
      nodes.Add(child);

      // add the node as a sub-object to the Dialogue
      AssetDatabase.AddObjectToAsset(child, this);

      // to redraw the GUI
      OnValidate();
    }

    /// <summary>
    /// Deletes the node
    /// </summary>
    /// <param name="nodeToDelete"></param>
    public void DeleteNode(DialogueNode nodeToDelete)
    {
      Undo.RecordObject(this, "Added Dialogue Node");

      nodes.Remove(nodeToDelete);

      OnValidate();

      // clean the children
      foreach (var dialogueNode in nodes)
      {
        dialogueNode.RemoveChild(nodeToDelete.name);
      }

      // immediately destroys the object
      // in Unity's C++ backend
      Undo.DestroyObjectImmediate(nodeToDelete);
    }

#endif

    /// <summary>
    /// called when about to save the asset to the Hard drive
    /// </summary>
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR

      var path = AssetDatabase.GetAssetPath(this);
      if (path == string.Empty) return;

      foreach (var dialogueNode in nodes)
      {
        if (AssetDatabase.GetAssetPath(dialogueNode) == string.Empty)
        {
          // add it to the asset database
          AssetDatabase.AddObjectToAsset(dialogueNode, this);
        }
      }

#endif
    }

    /// <summary>
    /// called when the file is loaded from the hard drive
    /// </summary>
    public void OnAfterDeserialize()
    {
    }

    public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
    {
      return GetAllChildren(currentNode).Where(dialogueNode => dialogueNode.IsPlayerSpeaking);
    }

    public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
    {
      return GetAllChildren(currentNode).Where(dialogueNode => !dialogueNode.IsPlayerSpeaking);
    }
  }
}