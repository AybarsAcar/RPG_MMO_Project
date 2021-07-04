using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
  public class Health : MonoBehaviour, ISavable
  {
    [SerializeField] private UnityEvent<float> takeDamage;
    [SerializeField] private UnityEvent onDie;

    private LazyValue<float> _health;
    public float HealthPoints => _health.Value;

    private bool _isDead;
    public bool IsDead => _isDead;

    private const float RegenerationPercentage = 70f;

    private void OnEnable()
    {
      GetComponent<BaseStats>().OnLevelUp += RegenerateHeath; // subscribe to Level up
    }

    private void Awake()
    {
      // initialise it the Health from the BaseStats
      _health = new LazyValue<float>(() => GetComponent<BaseStats>().GetStat(Stat.Health));
    }

    private void Start()
    {
      _health.ForceInit();
    }

    private void RegenerateHeath()
    {
      var regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (RegenerationPercentage / 100);

      _health.Value = Mathf.Max(_health.Value, regenHealthPoints);
    }

    /**
     * 
     */
    public void TakeDamage(GameObject instigator, float damage)
    {
      _health.Value = Mathf.Max(_health.Value - damage, 0);

      if (_health.Value <= 0)
      {
        onDie.Invoke(); // play the death sfx from Unity Event
        HandleDie();
        AwardExperience(instigator);
      }
      else
      {
        takeDamage.Invoke(damage);
      }
    }

    private void AwardExperience(GameObject instigator)
    {
      var experience = instigator.GetComponent<Experience>();
      if (experience == null) return;

      experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
    }

    public float GetNormalizedHealth()
    {
      return _health.Value / GetComponent<BaseStats>().GetStat(Stat.Health);
    }


    private void HandleDie()
    {
      if (_isDead) return;

      GetComponent<Animator>().SetTrigger("Die");
      _isDead = true;

      GetComponent<ActionScheduler>().CancelCurrentAction(); // stop the current action when character dies
    }

    public void Heal(float amountToHeal)
    {
      _health.Value = Mathf.Min(_health.Value + amountToHeal, GetMaxHealthPoints());
    }

    public object CaptureState()
    {
      return _health.Value; // float it is serializable by default since primitive data type
    }

    public void RestoreState(object state)
    {
      // restore the health points
      _health.Value = (float) state;

      // die
      if (_health.Value <= 0)
      {
        HandleDie();
      }
    }

    public float GetMaxHealthPoints()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }
  }
}