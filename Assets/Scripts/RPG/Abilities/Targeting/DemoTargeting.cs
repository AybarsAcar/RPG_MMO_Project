using System;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
  [CreateAssetMenu(fileName = "New Demo Targeting", menuName = "Abilities/Targeting Strategy/New Demo Targeting")]
  public class DemoTargeting : TargetingStrategy
  {
    public override void StartTargeting(AbilityData data, Action onFinish)
    {
      Debug.Log("Demo Targeting Strategy");
      
      onFinish();
    }
  }
}