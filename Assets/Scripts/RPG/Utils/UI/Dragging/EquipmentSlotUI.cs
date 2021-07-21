using RPG.Core.Util;
using RPG.Inventories;
using RPG.UI.Inventories;
using UnityEngine;

namespace RPG.Utils.UI.Dragging
{
  /// <summary>
  /// A slot for the players equipment.
  /// </summary>
  public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
  {
    // CONFIG DATA

    [SerializeField] private InventoryItemIcon icon = null;
    [SerializeField] private EquipLocation equipLocation = EquipLocation.Weapon;

    // CACHE
    private Equipment _playerEquipment;

    // LIFECYCLE METHODS

    private void Awake()
    {
      var player = GameObject.FindGameObjectWithTag(Tag.Player);
      _playerEquipment = player.GetComponent<Equipment>();
      _playerEquipment.EquipmentUpdated += RedrawUI;
    }

    private void Start()
    {
      RedrawUI();
    }

    // PUBLIC

    public int MaxAcceptable(InventoryItem item)
    {
      var equipableItem = item as EquipableItem;
      if (equipableItem == null) return 0;
      if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
      if (GetItem() != null) return 0;

      return 1;
    }

    public void AddItems(InventoryItem item, int number)
    {
      _playerEquipment.AddItem(equipLocation, (EquipableItem) item);
    }

    public InventoryItem GetItem()
    {
      return _playerEquipment.GetItemInSlot(equipLocation);
    }

    public int GetNumber()
    {
      if (GetItem() != null)
      {
        return 1;
      }
      else
      {
        return 0;
      }
    }

    public void RemoveItems(int number)
    {
      _playerEquipment.RemoveItem(equipLocation);
    }

    // PRIVATE

    private void RedrawUI()
    {
      icon.SetItem(_playerEquipment.GetItemInSlot(equipLocation));
    }
  }
}