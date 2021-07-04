using System;
using RPG.Core.Util;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Control
{
  /// <summary>
  /// Allows to player to pickup an item by running over it
  /// Item's collider.IsTrigger == true
  /// </summary>
  [RequireComponent(typeof(Pickup))]
  public class RunOverPickup : MonoBehaviour
  {
    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag(Tag.Player))
      {
        GetComponent<Pickup>().PickupItem();
      }
    }
  }
}