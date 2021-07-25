using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
  /// <summary>
  /// Responsible for handling the conversation with the AI
  /// Goes onto the Player Game Object
  /// </summary>
  public class PlayerConversant : MonoBehaviour
  {
    [SerializeField] private Dialogue currentDialogue;

    private DialogueNode _currentNode;

    // returns true if it's players turn to speak
    // choice buttons are displayed
    private bool _isPlayerChoosing;
    public bool IsPlayerChoosing => _isPlayerChoosing;

    private void Awake()
    {
      _currentNode = currentDialogue.GetRootNode();
    }
    
    /// <summary>
    /// returns the text property of the current node
    /// </summary>
    /// <returns></returns>
    public string GetText()
    {
      return _currentNode != null ? _currentNode.Text : string.Empty;
    }

    /// <summary>
    /// progresses the dialogue
    /// gets one of the children
    /// </summary>
    /// <returns></returns>
    public void Next()
    {
      var numOfPlayerResponses = currentDialogue.GetPlayerChildren(_currentNode).Count();

      if (numOfPlayerResponses > 0)
      {
        _isPlayerChoosing = true;
        return;
      }

      var nodes = currentDialogue.GetAIChildren(_currentNode).ToArray();

      _currentNode = nodes[Random.Range(0, nodes.Length)];
    }


    /// <summary>
    /// returns true if the current node has any children
    /// </summary>
    /// <returns></returns>
    public bool HasNext()
    {
      return currentDialogue.GetAllChildren(_currentNode).Any();
    }

    /// <summary>
    /// returns the player conversation options
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DialogueNode> GetChoices()
    {
      return currentDialogue.GetPlayerChildren(_currentNode);
    }

    /// <summary>
    /// selects a choice
    /// used to display its children
    /// </summary>
    /// <param name="chosenNode"></param>
    public void SelectChoice(DialogueNode chosenNode)
    {
      _currentNode = chosenNode;
      _isPlayerChoosing = false;
      
      // automatically advance to the next part in dialogue
      Next();
    }
  }
}