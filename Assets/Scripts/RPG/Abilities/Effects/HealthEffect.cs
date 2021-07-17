using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
  /// <summary>
  /// used for both dealing damage and healing
  /// </summary>
  [CreateAssetMenu(fileName = "New Health Effect", menuName = "Abilities/Effect Strategy/New Health Effect", order = 0)]
  public class HealthEffect : EffectStrategy
  {
    [Tooltip("Negative if damaging, positive if healing")] [SerializeField]
    private float healthChange;

    public override void StartEffect(AbilityData data, Action onFinish)
    {
      foreach (var target in data.Targets)
      {
        var health = target.GetComponent<Health>();

        if (!health) continue;

        if (healthChange < 0)
        {
          health.TakeDamage(data.User, -healthChange);
        }
        else
        {
          health.Heal(healthChange);
        }
      }

      onFinish();
    }
  }
}