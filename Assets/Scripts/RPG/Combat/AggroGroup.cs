using UnityEngine;

namespace RPG.Combat
{
  /// <summary>
  /// Tracks every enemy in the scene
  /// and triggers the aggro from a centralised component
  /// component sits on an enemies game object that groups all the enemies under it
  /// </summary>
  public class AggroGroup : MonoBehaviour
  {
    [SerializeField] private Fighter[] fighters;

    [SerializeField] private bool activateOnStart;

    private void Start()
    {
      Activate(activateOnStart);
    }

    public void Activate(bool shouldActivate)
    {
      foreach (var fighter in fighters)
      {
        var target = fighter.gameObject.GetComponent<CombatTarget>();

        if (target != null)
        {
          target.enabled = shouldActivate;
        }

        fighter.enabled = shouldActivate;
      }
    }
  }
}