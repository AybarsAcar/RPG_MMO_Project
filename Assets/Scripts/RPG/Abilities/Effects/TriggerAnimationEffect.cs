using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
  [CreateAssetMenu(fileName = "New Trigger Animation Effect", menuName = "Abilities/Effect Strategy/New Trigger Animation Effect",
    order = 0)]
  public class TriggerAnimationEffect : EffectStrategy
  {
    [SerializeField] private string animationTrigger;
    
    public override void StartEffect(AbilityData data, Action onFinish)
    {
      var animator = data.User.GetComponent<Animator>();
      
      animator.SetTrigger(animationTrigger);

      onFinish();
    }
  }
}