using System.Collections.Generic;
using RPG.Core;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
  /// <summary>
  /// An inventory item that can be equipped to the player. Weapons could be a
  /// subclass of this.
  /// </summary>
  [CreateAssetMenu(menuName = ("InventorySystem/New Equipable Item"))]
  public class EquipableItem : InventoryItem
  {
    // CONFIG DATA
    [Tooltip("Where are we allowed to put this item.")] [SerializeField]
    private EquipLocation allowedEquipLocation = EquipLocation.Weapon;

    [Tooltip("Conditions whether it can be equipped by the Player or not")] [SerializeField]
    private Condition equipCondition;

    // PUBLIC

    public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
    {
      if (equipLocation != allowedEquipLocation) return false;

      return equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
    }

    public EquipLocation GetAllowedEquipLocation()
    {
      return allowedEquipLocation;
    }
  }
}