using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
  /// <summary>
  /// Provides storage for the player inventory. A configurable number of
  /// slots are available.
  ///
  /// This component should be placed on the GameObject tagged "Player".
  /// </summary>
  public class Inventory : MonoBehaviour, IPredicateEvaluator, ISavable
  {
    // CONFIG DATA
    [Tooltip("Allowed size")] [SerializeField]
    private int inventorySize = 16;

    // STATE
    InventorySlot[] _slots;

    public struct InventorySlot
    {
      public InventoryItem Item;
      public int Number;
    }

    // PUBLIC

    /// <summary>
    /// Broadcasts when the items in the slots are added/removed.
    /// </summary>
    public event Action inventoryUpdated;

    /// <summary>
    /// Convenience for getting the player's inventory.
    /// </summary>
    public static Inventory GetPlayerInventory()
    {
      var player = GameObject.FindWithTag("Player");
      return player.GetComponent<Inventory>();
    }

    /// <summary>
    /// Could this item fit anywhere in the inventory?
    /// </summary>
    public bool HasSpaceFor(InventoryItem item)
    {
      return FindSlot(item) >= 0;
    }

    /// <summary>
    /// returns true if we have empty space
    /// make sure to take stackable items into consideration
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public bool HasSpaceFor(IEnumerable<InventoryItem> items)
    {
      var freeSlots = FreeSlotCount();
      var seenStackableItems = new List<InventoryItem>();

      foreach (var inventoryItem in items)
      {
        if (inventoryItem.IsStackable)
        {
          // already have the item in the inventory
          if (HasItem(inventoryItem)) continue;

          // or already seen it in the items list
          if (seenStackableItems.Contains(inventoryItem)) continue;

          seenStackableItems.Add(inventoryItem);
        }

        if (freeSlots <= 0) return false;

        freeSlots--;
      }

      return true;
    }

    /// <summary>
    /// inventorySlot.number == 0 means it's empty
    /// </summary>
    /// <returns>the number of free slots available in our inventory</returns>
    public int FreeSlotCount()
    {
      var count = 0;
      foreach (var inventorySlot in _slots)
      {
        if (inventorySlot.Number == 0)
        {
          count++;
        }
      }

      return count;
    }

    /// <summary>
    /// How many slots are in the inventory?
    /// </summary>
    public int GetSize()
    {
      return _slots.Length;
    }

    /// <summary>
    /// Attempt to add the items to the first available slot.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="number">The number to add.</param>
    /// <returns>Whether or not the item could be added.</returns>
    public bool AddToFirstEmptySlot(InventoryItem item, int number)
    {
      var i = FindSlot(item);

      if (i < 0)
      {
        return false;
      }

      _slots[i].Item = item;
      _slots[i].Number += number;
      if (inventoryUpdated != null)
      {
        inventoryUpdated();
      }

      return true;
    }

    /// <summary>
    /// Is there an instance of the item in the inventory?
    /// </summary>
    public bool HasItem(InventoryItem item)
    {
      for (int i = 0; i < _slots.Length; i++)
      {
        if (object.ReferenceEquals(_slots[i].Item, item))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Return the item type in the given slot.
    /// </summary>
    public InventoryItem GetItemInSlot(int slot)
    {
      return _slots[slot].Item;
    }

    /// <summary>
    /// Get the number of items in the given slot.
    /// </summary>
    public int GetNumberInSlot(int slot)
    {
      return _slots[slot].Number;
    }

    /// <summary>
    /// Remove a number of items from the given slot. Will never remove more
    /// that there are.
    /// </summary>
    public void RemoveFromSlot(int slot, int number)
    {
      _slots[slot].Number -= number;
      if (_slots[slot].Number <= 0)
      {
        _slots[slot].Number = 0;
        _slots[slot].Item = null;
      }

      if (inventoryUpdated != null)
      {
        inventoryUpdated();
      }
    }

    /// <summary>
    /// Will add an item to the given slot if possible. If there is already
    /// a stack of this type, it will add to the existing stack. Otherwise,
    /// it will be added to the first empty slot.
    /// </summary>
    /// <param name="slot">The slot to attempt to add to.</param>
    /// <param name="item">The item type to add.</param>
    /// <param name="number">The number of items to add.</param>
    /// <returns>True if the item was added anywhere in the inventory.</returns>
    public bool AddItemToSlot(int slot, InventoryItem item, int number)
    {
      if (_slots[slot].Item != null)
      {
        return AddToFirstEmptySlot(item, number);
        ;
      }

      var i = FindStack(item);
      if (i >= 0)
      {
        slot = i;
      }

      _slots[slot].Item = item;
      _slots[slot].Number += number;
      if (inventoryUpdated != null)
      {
        inventoryUpdated();
      }

      return true;
    }

    // PRIVATE

    private void Awake()
    {
      _slots = new InventorySlot[inventorySize];
    }

    /// <summary>
    /// Find a slot that can accomodate the given item.
    /// </summary>
    /// <returns>-1 if no slot is found.</returns>
    private int FindSlot(InventoryItem item)
    {
      int i = FindStack(item);
      if (i < 0)
      {
        i = FindEmptySlot();
      }

      return i;
    }

    /// <summary>
    /// Find an empty slot.
    /// </summary>
    /// <returns>-1 if all slots are full.</returns>
    private int FindEmptySlot()
    {
      for (int i = 0; i < _slots.Length; i++)
      {
        if (_slots[i].Item == null)
        {
          return i;
        }
      }

      return -1;
    }

    /// <summary>
    /// Find an existing stack of this item type.
    /// </summary>
    /// <returns>-1 if no stack exists or if the item is not stackable.</returns>
    private int FindStack(InventoryItem item)
    {
      if (!item.IsStackable)
      {
        return -1;
      }

      for (int i = 0; i < _slots.Length; i++)
      {
        if (object.ReferenceEquals(_slots[i].Item, item))
        {
          return i;
        }
      }

      return -1;
    }

    [System.Serializable]
    private struct InventorySlotRecord
    {
      public string itemID;
      public int number;
    }

    public bool? Evaluate(string predicate, string[] parameters)
    {
      return predicate switch
      {
        "HasInventoryItem" => HasItem(InventoryItem.GetFromID(parameters[0])),
        _ => null
      };
    }

    object ISavable.CaptureState()
    {
      var slotStrings = new InventorySlotRecord[inventorySize];
      for (int i = 0; i < inventorySize; i++)
      {
        if (_slots[i].Item == null) continue;

        slotStrings[i].itemID = _slots[i].Item.ItemId;
        slotStrings[i].number = _slots[i].Number;
      }

      return slotStrings;
    }

    void ISavable.RestoreState(object state)
    {
      var slotStrings = (InventorySlotRecord[]) state;
      for (int i = 0; i < inventorySize; i++)
      {
        _slots[i].Item = InventoryItem.GetFromID(slotStrings[i].itemID);
        _slots[i].Number = slotStrings[i].number;
      }

      if (inventoryUpdated != null)
      {
        inventoryUpdated();
      }
    }
  }
}