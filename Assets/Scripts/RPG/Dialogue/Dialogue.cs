using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
  [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
  public class Dialogue : ScriptableObject
  {
    [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

#if UNITY_EDITOR
    private void Awake()
    {
      // add a default node
      nodes.Add(new DialogueNode());
    }
#endif

    public IEnumerable<DialogueNode> GetAllNodes()
    {
      return nodes;
    }

    public DialogueNode GetRootNode()
    {
      return nodes[0];
    }
  }
}