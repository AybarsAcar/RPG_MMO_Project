using System;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Control
{
  /// <summary>
  /// allows the player to pick up an item by clicking on the item
  /// </summary>
  [RequireComponent(typeof(Pickup))]
  public class ClickablePickup : MonoBehaviour, IRaycastable
  {
    private Pickup _pickup;

    private void Awake()
    {
      _pickup = GetComponent<Pickup>();
    }

    public bool HandleRaycast(PlayerController callingController)
    {
      if (Input.GetMouseButton(0))
      {
        _pickup.PickupItem();
      }

      return true;
    }

    public CursorType GetCursorType()
    {
      return _pickup.CanBePickedUp() ? CursorType.Pickup : CursorType.None;
    }
    
    // TODO: add a pickup radius
  }
}