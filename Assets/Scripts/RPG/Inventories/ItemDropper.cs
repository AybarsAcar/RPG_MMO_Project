using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
  /// <summary>
  /// To be placed on anything that wishes to drop pickups into the world.
  /// Tracks the drops for saving and restoring.
  /// </summary>
  public class ItemDropper : MonoBehaviour, ISavable
  {
    // STATE
    private List<Pickup> droppedItems = new List<Pickup>();
    private List<DropRecord> otherSceneDropRecords = new List<DropRecord>();

    // PUBLIC

    /// <summary>
    /// Create a pickup at the current position.
    /// </summary>
    /// <param name="item">The item type for the pickup.</param>
    /// <param name="number">
    /// The number of items contained in the pickup. Only used if the item
    /// is stackable.
    /// </param>
    public void DropItem(InventoryItem item, int number)
    {
      SpawnPickup(item, GetDropLocation(), number);
    }

    /// <summary>
    /// Create a pickup at the current position.
    /// </summary>
    /// <param name="item">The item type for the pickup.</param>
    public void DropItem(InventoryItem item)
    {
      SpawnPickup(item, GetDropLocation(), 1);
    }

    // PROTECTED

    /// <summary>
    /// Override to set a custom method for locating a drop.
    /// </summary>
    /// <returns>The location the drop should be spawned.</returns>
    protected virtual Vector3 GetDropLocation()
    {
      return transform.position;
    }

    // PRIVATE

    public void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
    {
      var pickup = item.SpawnPickup(spawnLocation, number);
      droppedItems.Add(pickup);
    }

    [System.Serializable]
    private struct DropRecord
    {
      public string itemID;
      public SerializableVector3 position;
      public int number;
      public int sceneBuildIndex;
    }

    object ISavable.CaptureState()
    {
      RemoveDestroyedDrops();

      var droppedItemsList = new List<DropRecord>();
      var currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

      foreach (var pickup in droppedItems)
      {
        var droppedItem = new DropRecord
        {
          itemID = pickup.Item.ItemId,
          position = new SerializableVector3(pickup.transform.position),
          number = pickup.Number,
          sceneBuildIndex = currentSceneBuildIndex
        };

        droppedItemsList.Add(droppedItem);
      }

      // merge other pickups in other scenes
      // so no information is lost
      droppedItemsList.AddRange(otherSceneDropRecords);
      
      return droppedItemsList;
    }

    void ISavable.RestoreState(object state)
    {
      var droppedItemsList = (List<DropRecord>) state;

      var currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
      
      otherSceneDropRecords.Clear();

      foreach (var item in droppedItemsList)
      {
        if (item.sceneBuildIndex != currentSceneBuildIndex)
        {
          otherSceneDropRecords.Add(item);
          continue;
        }
        
        var pickupItem = InventoryItem.GetFromID(item.itemID);
        var position = item.position.ToVector3();
        var number = item.number;
        SpawnPickup(pickupItem, position, number);
      }
    }

    /// <summary>
    /// Remove any drops in the world that have subsequently been picked up.
    /// </summary>
    private void RemoveDestroyedDrops()
    {
      var newList = new List<Pickup>();
      foreach (var item in droppedItems)
      {
        if (item != null)
        {
          newList.Add(item);
        }
      }

      droppedItems = newList;
    }
  }
}