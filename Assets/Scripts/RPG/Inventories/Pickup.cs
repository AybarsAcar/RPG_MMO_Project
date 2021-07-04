using UnityEngine;

namespace RPG.Inventories
{
  /// <summary>
  /// To be placed at the root of a Pickup prefab. Contains the data about the
  /// pickup such as the type of item and the number.
  /// </summary>
  public class Pickup : MonoBehaviour
  {
    // STATE
    InventoryItem _item;
    public InventoryItem Item => _item;

    int _number = 1;
    public int Number => _number;

    // CACHED REFERENCE
    Inventory _inventory;

    // LIFECYCLE METHODS

    private void Awake()
    {
      var player = GameObject.FindGameObjectWithTag("Player");
      _inventory = player.GetComponent<Inventory>();
    }

    // PUBLIC

    /// <summary>
    /// Set the vital data after creating the prefab.
    /// </summary>
    /// <param name="item">The type of item this prefab represents.</param>
    /// <param name="number">The number of items represented.</param>
    public void Setup(InventoryItem item, int number)
    {
      this._item = item;
      if (!item.IsStackable)
      {
        number = 1;
      }

      this._number = number;
    }

    public void PickupItem()
    {
      var foundSlot = _inventory.AddToFirstEmptySlot(_item, _number);
      if (foundSlot)
      {
        Destroy(gameObject);
      }
    }

    public bool CanBePickedUp()
    {
      return _inventory.HasSpaceFor(_item);
    }
  }
}