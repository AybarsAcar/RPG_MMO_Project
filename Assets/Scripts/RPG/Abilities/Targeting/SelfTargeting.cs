using System;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
  [CreateAssetMenu(fileName = "New Self Targeting", menuName = "Abilities/Targeting Strategy/New Self Targeting")]
  public class SelfTargeting : TargetingStrategy
  {
    public override void StartTargeting(AbilityData data, Action onFinish)
    {
      // set the target to the user
      data.Targets = new[] {data.User};
      data.TargetedPoint = data.User.transform.position;

      onFinish();
    }
  }
}