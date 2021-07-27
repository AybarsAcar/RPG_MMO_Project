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
    public UnityEvent onDie;

    private LazyValue<float> _health;
    public float HealthPoints => _health.Value;

    public bool _wasDeadLastFrame;
    public bool IsDead => _health.Value <= 0;

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

    /// <summary>
    /// Handles taking damage
    /// </summary>
    /// <param name="instigator">Damage Dealer</param>
    /// <param name="damage">Damage Amount</param>
    public void TakeDamage(GameObject instigator, float damage)
    {
      _health.Value = Mathf.Max(_health.Value - damage, 0);

      if (IsDead)
      {
        onDie.Invoke(); // play the death sfx from Unity Event
        AwardExperience(instigator);
      }
      else
      {
        takeDamage.Invoke(damage);
      }

      UpdateState();
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


    private void UpdateState()
    {
      var animator = GetComponent<Animator>();

      if (!_wasDeadLastFrame && IsDead)
      {
        animator.SetTrigger("Die");
        GetComponent<ActionScheduler>().CancelCurrentAction(); // stop the current action when character dies        
      }

      if (_wasDeadLastFrame && !IsDead)
      {
        // revived
        animator.Rebind();
      }

      _wasDeadLastFrame = IsDead;
    }

    public void Heal(float amountToHeal)
    {
      _health.Value = Mathf.Min(_health.Value + amountToHeal, GetMaxHealthPoints());

      UpdateState();
    }

    public object CaptureState()
    {
      return _health.Value; // float it is serializable by default since primitive data type
    }

    public void RestoreState(object state)
    {
      // restore the health points
      _health.Value = (float) state;

      UpdateState();
    }

    public float GetMaxHealthPoints()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }
  }
}