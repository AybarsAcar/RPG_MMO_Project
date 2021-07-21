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
  public class Fighter : MonoBehaviour, IAction
  {
    // equals null so the compiler knows its deliberately null
    [SerializeField] private Transform rightHandTransform = null;
    [SerializeField] private Transform leftHandTransform = null;
    [SerializeField] private WeaponConfig defaultWeaponConfig;
    [SerializeField] private float autoAttackRange = 4f;

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
        _equipment.EquipmentUpdated += UpdateWeapon;
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

      if (_target.IsDead)
      {
        // auto attack behaviour
        _target = FindNewTargetInRange();

        // if no target in range stop
        if (_target == null) return;
      }

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
    /// finds a target in range when the user dies
    /// for Auto attacking behaviour
    /// </summary>
    /// <returns>the closest enemy</returns>
    private Health FindNewTargetInRange()
    {
      Health enemyToAttack = null;
      var enemyDistance = Mathf.Infinity;

      foreach (var enemy in FindAllTargetsInRange())
      {
        var distance = Vector3.Distance(transform.position, enemy.transform.position);

        if (distance < enemyDistance)
        {
          enemyDistance = distance;
          enemyToAttack = enemy;
        }
      }

      return enemyToAttack;
    }

    private IEnumerable<Health> FindAllTargetsInRange()
    {
      // Sphere cast around the player current location 
      var hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up, 0);

      // and look for Health component player can attack
      foreach (var hit in hits)
      {
        var health = hit.transform.GetComponent<Health>();

        if (health == null) continue;

        if (health.IsDead) continue;

        // our player
        if (health.gameObject == gameObject) continue;

        yield return health;
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
    /// gets the active hand
    /// </summary>
    /// <returns></returns>
    public Transform GetHandTransform(bool isRightHand)
    {
      return isRightHand ? rightHandTransform : leftHandTransform;
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

    /// <summary>
    /// Animation Event
    /// this is by adding an animation key so we trigger this at that point 
    /// this is the melee attack animation event
    /// </summary>
    private void Hit()
    {
      if (_target == null) return;

      var damage = GetComponent<BaseStats>().GetStat(Stat.Damage); // damage from our stats based on Level

      var targetBaseStats = _target.GetComponent<BaseStats>();

      if (targetBaseStats != null)
      {
        var defence = targetBaseStats.GetStat(Stat.Defence);
        damage /= 1 + defence / damage;
      }


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

    /// <summary>
    /// Animation Event
    /// this is by adding an animation key so we trigger this at that point
    /// this is the ranged attack animation event
    /// </summary>
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
  }
}