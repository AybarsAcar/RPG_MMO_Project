using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
  /// <summary>
  /// holds which action it responds to
  /// and what it does when that action is triggered
  /// </summary>
  [RequireComponent(typeof(AIConversant))]
  public class DialogueTrigger : MonoBehaviour
  {
    [SerializeField] private string action;
    [SerializeField] private UnityEvent onTrigger;

    /// <summary>
    /// calls by the unity event
    /// </summary>
    /// <param name="actionToTrigger"></param>
    public void Trigger(string actionToTrigger)
    {
      if (actionToTrigger == action)
      {
        onTrigger.Invoke(); // invokes the Unity Event
      }
    }
  }
}