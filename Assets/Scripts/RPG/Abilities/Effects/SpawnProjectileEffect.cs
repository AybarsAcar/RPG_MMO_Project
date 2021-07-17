using System;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Abilities.Effects
{
  [CreateAssetMenu(fileName = "New Spawn Projectile Effect",
    menuName = "Abilities/Effect Strategy/New Spawn Projectile Effect",
    order = 0)]
  public class SpawnProjectileEffect : EffectStrategy
  {
    [SerializeField] private Projectile projectileToSpawn;
    [SerializeField] private float damage;
    [SerializeField] private bool isRightHand = true;

    [Tooltip("True if skill shot")] [SerializeField]
    private bool useTargetPoint = true;

    public override void StartEffect(AbilityData data, Action onFinish)
    {
      var fighter = data.User.GetComponent<Fighter>();
      var spawnPosition = fighter.GetHandTransform(isRightHand).transform.position;

      if (useTargetPoint)
      {
        SpawnProjectileForTargetPoint(data, spawnPosition);
      }
      else
      {
        SpawnProjectileForTargets(data, spawnPosition);
      }


      onFinish();
    }

    /// <summary>
    /// skill shot
    /// targeted projectile
    /// </summary>
    /// <param name="data"></param>
    /// <param name="spawnPosition"></param>
    private void SpawnProjectileForTargetPoint(AbilityData data, Vector3 spawnPosition)
    {
      var projectile = Instantiate(projectileToSpawn);
      projectile.transform.position = spawnPosition;

      projectile.SetTarget(targetPoint: data.TargetedPoint, data.User, damage);
    }

    /// <summary>
    /// not a skill shot, click ability
    /// </summary>
    /// <param name="data"></param>
    /// <param name="spawnPosition"></param>
    private void SpawnProjectileForTargets(AbilityData data, Vector3 spawnPosition)
    {
      foreach (var target in data.Targets)
      {
        var targetHealth = target.GetComponent<Health>();
        if (!targetHealth) continue;

        var projectile = Instantiate(projectileToSpawn);
        projectile.transform.position = spawnPosition;

        projectile.SetTarget(targetHealth, data.User, damage);
      }
    }
  }
}