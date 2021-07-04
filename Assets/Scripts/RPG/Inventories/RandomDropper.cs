using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
  /// <summary>
  /// inherits from the ItemDropper MonoBehaviour
  /// used on the Enemies to drop the items on them when killed
  /// TODO: implement enemies dropping their current yielded weapon
  /// </summary>
  public class RandomDropper : ItemDropper
  {
    [Tooltip("How far can the pickups be scattered from the dropper")] [SerializeField]
    private float scatterDistance = 1f;

    [SerializeField] private DropLibrary dropLibrary;

    // number of attempts to hit the navmesh
    private const int Attempts = 30;

    /// <summary>
    /// randomly drops an item
    /// called by the Death Script on the enemy prefab
    /// </summary>
    public void RandomDrop()
    {
      var baseStats = GetComponent<BaseStats>();
      var drops = dropLibrary.GetRandomDrops(baseStats.GetPlayerLevel());

      foreach (var drop in drops)
      {
        DropItem(drop.item, drop.number);
      }
    }

    /// <summary>
    /// drops in a random location
    /// to scatter the items around the enemy
    /// </summary>
    /// <returns>a random location</returns>
    protected override Vector3 GetDropLocation()
    {
      for (int i = 0; i < Attempts; i++)
      {
        var randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;

        // make sure it drops on a NavMesh
        if (NavMesh.SamplePosition(randomPoint, out var hit, 0.1f, NavMesh.AllAreas))
        {
          return hit.position;
        }
      }

      // fallback if it doesn't hit the navmesh in all Attempts
      return transform.position;
    }
  }
}