using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
  /// <summary>
  /// Faces to the target when animating
  /// </summary>
  [CreateAssetMenu(fileName = "New Orient Target Effect",
    menuName = "Abilities/Effect Strategy/New Orient Target Effect",
    order = 0)]
  public class OrientToTargetEffect : EffectStrategy
  {
    public override void StartEffect(AbilityData data, Action onFinish)
    {
      data.User.transform.LookAt(data.TargetedPoint);

      onFinish();
    }
  }
}