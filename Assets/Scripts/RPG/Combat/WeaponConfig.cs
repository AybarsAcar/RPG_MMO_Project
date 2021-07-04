using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Combat
{
  /// <summary>
  /// Special Equipable for weapon
  /// </summary>
  [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
  public class WeaponConfig : EquipableItem, IModifierProvider
  {
    [SerializeField] private float range = 1.5f;
    public float Range => range;

    [SerializeField] private float flatDamageBonus = 10f;

    [SerializeField] private float percentageBonus = 0f;

    [SerializeField] private float timeBetweenAttacks = 1f;
    public float TimeBetweenAttacks => timeBetweenAttacks;

    [SerializeField] private bool isRightHanded = true;

    [SerializeField] private Weapon equippedPrefab = null;
    [SerializeField] private AnimatorOverrideController animatorOverride = null;
    [SerializeField] private Projectile projectile = null;

    private const string WeaponName = "Weapon";

    /**
     * instantiates the weapon in the player hand
     * and overrides the default animator
     */
    public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
    { 
      DestroyOldWeapon(rightHand, leftHand);

      Weapon weapon = null;
      
      if (equippedPrefab != null)
      {
        var hand = isRightHanded ? rightHand : leftHand;

        weapon = Instantiate(equippedPrefab, hand);
        weapon.gameObject.name = WeaponName;
      }

      if (animatorOverride != null)
      {
        animator.runtimeAnimatorController = animatorOverride;
      }
      else
      {
        // will be null if there is no override
        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

        if (overrideController != null)
        {
          animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }
      }

      return weapon;
    }

    private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
    {
      var oldWeapon = rightHand.Find(WeaponName);
      if (oldWeapon == null)
      {
        oldWeapon = leftHand.Find(WeaponName);
      }

      if (oldWeapon == null)
      {
        return;
      }

      // to make sure we are not destroying the recently picked up weapon
      oldWeapon.name = "DESTROYING";
      Destroy(oldWeapon.gameObject);
    }

    public bool HasProjectile()
    {
      return projectile != null;
    }

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
    {
      var hand = isRightHanded ? rightHand : leftHand;

      var projectileInstance = Instantiate(projectile, hand.position, Quaternion.identity);

      projectileInstance.SetTarget(target, instigator, calculatedDamage);
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
      if (stat == Stat.Damage)
      {
        yield return flatDamageBonus;
      }
    }

    public IEnumerable<float> GetPercentageModifier(Stat stat)
    {
      if (stat == Stat.Damage)
      {
        yield return percentageBonus;
      }
    }
  }
}