using RPG.Attributes;
using RPG.Core;
using RPG.Utils.UI.Dragging.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
  [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/New Ability")]
  public class Ability : ActionItem
  {
    [SerializeField] private float cooldown;
    [SerializeField] private float manaCost;

    [SerializeField] private TargetingStrategy targetingStrategy;
    [SerializeField] private FilterStrategy[] filterStrategies;
    [SerializeField] private EffectStrategy[] effectStrategies;

    public override bool Use(GameObject user)
    {
      var cooldownStore = user.GetComponent<CooldownStore>();
      var mana = user.GetComponent<Mana>();

      if (cooldownStore.GetCooldownTimeRemaining(this) > 0)
      {
        // cooldown in progress
        return false;
      }

      if (mana.CurrentMana < manaCost)
      {
        // not enough mana
        return false;
      }

      var data = new AbilityData {User = user};

      // start the ability as an action so it stops the other actions
      var actionScheduler = user.GetComponent<ActionScheduler>();
      actionScheduler.StartAction(data);

      targetingStrategy.StartTargeting(data, () => TargetAcquired(data));

      return true;
    }

    private void TargetAcquired(AbilityData data)
    {
      if (data.IsCancelled) return;

      // start the cooldown timer
      var cooldownStore = data.User.GetComponent<CooldownStore>();
      var mana = data.User.GetComponent<Mana>();

      // consume mana
      // if problem consuming mana return early
      if (!mana.UseMana(manaCost)) return;

      cooldownStore.StartCooldown(this, cooldown);

      foreach (var filterStrategy in filterStrategies)
      {
        data.Targets = filterStrategy.Filter(data.Targets);
      }

      foreach (var effectStrategy in effectStrategies)
      {
        effectStrategy.StartEffect(data, () => { });
      }
    }
  }
}