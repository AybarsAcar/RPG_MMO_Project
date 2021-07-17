using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
  /// <summary>
  /// this class is used to instantiate particle effects with abilities
  /// </summary>
  [CreateAssetMenu(fileName = "New Spawn Target Prefab", menuName = "Abilities/Effect Strategy/New Spawn Target Prefab",
    order = 0)]
  public class SpawnTargetPrefab : EffectStrategy
  {
    [SerializeField] private Transform prefabToSpawn;
    [SerializeField] private float destroyDelay = -1; // do not destroy unless specified otherwise

    public override void StartEffect(AbilityData data, Action onFinish)
    {
      data.StartCoroutine(Effect(data, onFinish));
    }

    private IEnumerator Effect(AbilityData data, Action onFinish)
    {
      var instance = Instantiate(prefabToSpawn);
      instance.position = data.TargetedPoint;

      if (destroyDelay > 0)
      {
        // wait for the delay and destrow
        yield return new WaitForSeconds(destroyDelay);
        Destroy(instance.gameObject);
      }

      onFinish();
    }
  }
}