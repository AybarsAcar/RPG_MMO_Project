using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction, ISavable
  {
    // equals null so the compiler knows its deliberately null
    [SerializeField] private Transform rightHandTransform = null;
    [SerializeField] private Transform leftHandTransform = null;
    [SerializeField] private WeaponConfig defaultWeaponConfig;

    private Health _target;
    public Health Target => _target;

    private Equipment _equipment;

    private Animator _animator;
    private Mover _mover;
    private ActionScheduler _actionScheduler;

    private float _timeSinceLastAttack = Mathf.Infinity;

    private WeaponConfig _currentWeaponConfig = null;
    private LazyValue<Weapon> _currentWeapon;

    private void Awake()
    {
      _mover = GetComponent<Mover>();
      _animator = GetComponent<Animator>();
      _actionScheduler = GetComponent<ActionScheduler>();

      _currentWeaponConfig = defaultWeaponConfig;
      _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

      _equipment = GetComponent<Equipment>();

      if (_equipment)
      {
        _equipment.equipmentUpdated += UpdateWeapon;
      }
    }

    private void Start()
    {
      _currentWeapon.ForceInit();
    }

    private Weapon SetupDefaultWeapon()
    {
      return EquipWeapon(defaultWeaponConfig);
    }

    private void Update()
    {
      _timeSinceLastAttack += Time.deltaTime;

      if (_target == null) return;

      if (_target.IsDead) return;

      if (_target && !IsInRange(_target.transform))
      {
        _mover.MoveTo(_target.transform.position, 1f);
      }
      else
      {
        // player in range so stop and attack
        _mover.Cancel();
        AttackBehavior();
      }
    }

    /// <summary>
    /// Equips weapons
    /// its called on pickups
    /// </summary>
    /// <param name="weaponConfig">Weapon Config Data - Equipable</param>
    /// <returns>the Weapon itself</returns>
    public Weapon EquipWeapon(WeaponConfig weaponConfig)
    {
      _currentWeaponConfig = weaponConfig;

      _currentWeapon.Value = weaponConfig.Spawn(rightHandTransform, leftHandTransform, _animator);

      return _currentWeapon.Value;
    }
    
    
    /// <summary>
    /// Updates the weapon as 
    /// </summary>
    private void UpdateWeapon()
    {
      // get the weapon from Equipment component
      // we grab the weapon from the weapon slot in our Equipment HUD
      var weapon = (WeaponConfig) _equipment.GetItemInSlot(EquipLocation.Weapon);

      if (weapon == null)
      {
        EquipWeapon(defaultWeaponConfig);
      }
      else
      {
        EquipWeapon(weapon);
      }
    }

    public bool CanAttack(GameObject combatTarget)
    {
      if (combatTarget == null) return false;

      if (!_mover.CanMoveTo(combatTarget.transform.position) && !IsInRange(combatTarget.transform)) return false;

      var targetHealth = combatTarget.GetComponent<Health>();

      return targetHealth != null && !targetHealth.IsDead;
    }

    private void AttackBehavior()
    {
      if (_timeSinceLastAttack >= _currentWeaponConfig.TimeBetweenAttacks) // this is the cooldown
      {
        transform.LookAt(_target.transform); // make sure we always face the enemy when basic attacking

        _animator.ResetTrigger("StopAttack");
        _animator.SetTrigger("Attack"); // this will trigger the Hit Animation Event

        _timeSinceLastAttack = 0;
      }
    }


    /**
       * Animation Event
       * this is by adding an animation key so we trigger this at that point
       * this is the melee attack animation event
       */
    private void Hit()
    {
      if (_target == null) return;

      var damage = GetComponent<BaseStats>().GetStat(Stat.Damage); // damage from our stats based on Level

      if (_currentWeapon.Value != null)
      {
        _currentWeapon.Value.OnHit();
      }
      
      if (_currentWeaponConfig.HasProjectile())
      {
        _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
      }
      else
      {
        _target.TakeDamage(gameObject, damage);
      }
    }

    /**
       * Animation Event
       * this is by adding an animation key so we trigger this at that point
       * this is the ranged attack animation event
       */
    private void Shoot()
    {
      Hit();
    }

    public void Attack(GameObject target)
    {
      _actionScheduler.StartAction(this);
      _target = target.GetComponent<Health>();
    }

    public void Cancel()
    {
      _animator.ResetTrigger("Attack");
      _animator.SetTrigger("StopAttack");

      _target = null;
      _mover.Cancel();
    }


    private bool IsInRange(Transform targetTransform)
    {
      return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.Range;
    }

    public object CaptureState()
    {
      return _currentWeaponConfig.name;
    }

    public void RestoreState(object state)
    {
      var weaponName = (string) state;
      var weapon = Resources.Load<WeaponConfig>(weaponName);

      EquipWeapon(weapon);
    }
  }
}