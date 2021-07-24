using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
  [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
  public class Dialogue : ScriptableObject
  {
    [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

    private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
    private void Awake()
    {
      if (nodes.Count == 0)
      {
        var rootNode = new DialogueNode {id = Guid.NewGuid().ToString()};

        // add a default node
        nodes.Add(rootNode);
      }

      // so its called when built as well
      OnValidate();
    }
#endif

    /// <summary>
    /// called when the values is changed or the script is loaded
    /// </summary>
    private void OnValidate()
    {
      _nodeLookup.Clear();

      foreach (var dialogueNode in nodes)
      {
        _nodeLookup[dialogueNode.id] = dialogueNode;
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
      foreach (var childId in parentNode.childIds)
      {
        if (_nodeLookup.ContainsKey(childId))
        {
          yield return _nodeLookup[childId];
        }
      }
    }

    /// <summary>
    /// Creates a Dialogue node as a child
    /// </summary>
    /// <param name="parent">Parent node of the newly created node</param>
    public void CreateNode(DialogueNode parent)
    {
      var child = new DialogueNode {id = Guid.NewGuid().ToString()};
      
      parent.childIds.Add(child.id);
      
      // added to the global nodes
      nodes.Add(child);
      
      // to redraw the GUI
      OnValidate();
    }

    /// <summary>
    /// Deletes the node
    /// </summary>
    /// <param name="nodeToDelete"></param>
    public void DeleteNode(DialogueNode nodeToDelete)
    {
      nodes.Remove(nodeToDelete);
      
      OnValidate();

      // clean the children
      foreach (var dialogueNode in nodes)
      {
        dialogueNode.childIds.Remove(nodeToDelete.id);
      }
    }
  }
}