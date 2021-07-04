using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
  [RequireComponent(typeof(Health))]
  public class CombatTarget : MonoBehaviour, IRaycastable
  {
    /**
     * if the method is unsuccessful the player will move instead
     * this method will also be used to change the look of our cursor to tell the player that the
     * game object the cursor is hovering is attackable
     * gameObject is the object being attacked
     * callingController is the game object attacking 
     */
    public bool HandleRaycast(PlayerController callingController)
    {
      if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
      {
        return false;
      }

      if (Input.GetMouseButton(1))
      {
        callingController.GetComponent<Fighter>().Attack(gameObject);
      }

      return true;
    }

    public CursorType GetCursorType()
    {
      return CursorType.Combat;
    }
  }
}