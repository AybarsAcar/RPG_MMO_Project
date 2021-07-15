using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
  [CreateAssetMenu(fileName = "New Demo Targeting", menuName = "Abilities/Targeting Strategy/New Demo Targeting")]
  public class DemoTargeting : TargetingStrategy
  {
    public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> onFinish)
    {
      Debug.Log("Demo Targeting Strategy");
      
      onFinish(null);
    }
  }
}