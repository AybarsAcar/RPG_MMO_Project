using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
  /// <summary>
  /// aggregate class
  /// </summary>
  [CreateAssetMenu(fileName = "New Delay Composite Effect",
    menuName = "Abilities/Effect Strategy/New Delay Composite Effect",
    order = 0)]
  public class DelayCompositeEffect : EffectStrategy
  {
    [Tooltip("Delay Time in Seconds")] [SerializeField]
    private float delayTime;

    [Tooltip("Effects that requires to be delayed")] [SerializeField]
    private EffectStrategy[] delayedEffects;

    [Tooltip("Set to true if the ability can be cancelable after trigger the effects")] [SerializeField]
    private bool abortIfCancelled;

    public override void StartEffect(AbilityData data, Action onFinish)
    {
      data.StartCoroutine(DelayedEffects(data, onFinish));
    }

    private IEnumerator DelayedEffects(AbilityData data, Action onFinish)
    {
      yield return new WaitForSeconds(delayTime);

      if (abortIfCancelled && data.IsCancelled) yield break;

      foreach (var effect in delayedEffects)
      {
        effect.StartEffect(data, onFinish);
      }
    }
  }
}