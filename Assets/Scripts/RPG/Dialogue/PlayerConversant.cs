using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
  /// <summary>
  /// Responsible for handling the conversation with the AI
  /// Goes onto the Player Game Object
  /// </summary>
  public class PlayerConversant : MonoBehaviour
  {
    [SerializeField] private string displayName;
    
    private Dialogue _currentDialogue;
    private DialogueNode _currentNode;
    private AIConversant _currentAiConversant;

    // called each time there is a change to the conversation
    // subscribed form the DialogueUI and triggers RefreshUI
    public event Action OnConversationUpdated;

    // returns true if it's players turn to speak
    // choice buttons are displayed
    private bool _isPlayerChoosing;
    public bool IsPlayerChoosing => _isPlayerChoosing;

    public bool IsActive()
    {
      return _currentDialogue != null;
    }

    /// <summary>
    /// Starts the dialogue in the event
    /// </summary>
    /// <param name="dialogue"></param>
    public void StartDialogue(AIConversant currentAiConversant, Dialogue dialogue)
    {
      _currentAiConversant = currentAiConversant;
      _currentDialogue = dialogue;
      _currentNode = _currentDialogue.GetRootNode();

      TriggerEnterAction();

      OnConversationUpdated?.Invoke();
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
      var numOfPlayerResponses = _currentDialogue.GetPlayerChildren(_currentNode).Count();

      if (numOfPlayerResponses > 0)
      {
        _isPlayerChoosing = true;

        TriggerExitAction();

        OnConversationUpdated?.Invoke();
        return;
      }

      var nodes = _currentDialogue.GetAIChildren(_currentNode).ToArray();

      TriggerExitAction();

      _currentNode = nodes[Random.Range(0, nodes.Length)];

      TriggerEnterAction();

      OnConversationUpdated?.Invoke();
    }


    /// <summary>
    /// returns true if the current node has any children
    /// </summary>
    /// <returns></returns>
    public bool HasNext()
    {
      return _currentDialogue.GetAllChildren(_currentNode).Any();
    }

    /// <summary>
    /// returns the player conversation options
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DialogueNode> GetChoices()
    {
      return _currentDialogue.GetPlayerChildren(_currentNode);
    }

    /// <summary>
    /// selects a choice
    /// used to display its children
    /// </summary>
    /// <param name="chosenNode"></param>
    public void SelectChoice(DialogueNode chosenNode)
    {
      _currentNode = chosenNode;

      TriggerEnterAction();

      _isPlayerChoosing = false;

      // automatically advance to the next part in dialogue
      Next();
    }

    /// <summary>
    /// sets the current dialogue and node states to null
    /// </summary>
    public void Quit()
    {
      _currentDialogue = null;

      TriggerExitAction();

      _currentNode = null;
      _isPlayerChoosing = false;

      _currentAiConversant = null;

      OnConversationUpdated?.Invoke();
    }

    private void TriggerEnterAction()
    {
      if (_currentNode != null)
      {
        TriggerAction(_currentNode.OnEnterAction);
      }
    }

    private void TriggerExitAction()
    {
      if (_currentNode != null)
      {
        TriggerAction(_currentNode.OnExitAction);
      }
    }

    private void TriggerAction(string action)
    {
      if (action == string.Empty) return;

      foreach (var trigger in _currentAiConversant.GetComponents<DialogueTrigger>())
      {
        trigger.Trigger(action);
      }
    }

    public string GetCurrentConversantName()
    {
      if (_isPlayerChoosing)
      {
        return displayName != string.Empty ? displayName : "Player";
      }
      
      return _currentAiConversant.DisplayName != string.Empty ? _currentAiConversant.DisplayName : "Default Enemy";
    }
  }
}