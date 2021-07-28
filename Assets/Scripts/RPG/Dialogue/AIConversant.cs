using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
  public class AIConversant : MonoBehaviour, IRaycastable
  {
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private string displayName; // name of the conversant
    public string DisplayName => displayName;

    public bool HandleRaycast(PlayerController callingController)
    {
      if (dialogue == null) return false;

      var health = GetComponent<Health>();
      if (health != null && health.IsDead) return false;

      if (Input.GetMouseButtonUp(0))
      {
        callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
      }

      return true;
    }

    public CursorType GetCursorType()
    {
      return CursorType.Dialogue;
    }
  }
}