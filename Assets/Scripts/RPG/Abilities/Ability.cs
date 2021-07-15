using System.Collections.Generic;
using RPG.Utils.UI.Dragging.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
  [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/New Ability")]
  public class Ability : ActionItem
  {
    [SerializeField] private TargetingStrategy targetingStrategy;

    public override void Use(GameObject user)
    {
      targetingStrategy.StartTargeting(user, TargetAcquired);
    }

    private void TargetAcquired(IEnumerable<GameObject> targets)
    {
      foreach (var target in targets)
      {
        Debug.Log(target.name);
      }
    }
  }
}