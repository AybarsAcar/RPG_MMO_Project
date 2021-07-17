using System;
using RPG.Abilities;
using RPG.Core.Util;
using RPG.Inventories;
using RPG.Utils.UI.Dragging;
using RPG.Utils.UI.Dragging.Inventories;
using UnityEngine;
using UnityEngine.UI;
using InventoryItem = RPG.Inventories.InventoryItem;

namespace RPG.UI.Inventories
{
  /// <summary>
  /// The UI slot for the player action bar.
  /// </summary>
  public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
  {
    // CONFIG DATA
    [SerializeField] private InventoryItemIcon icon = null;
    [SerializeField] private int index = 0;
    [SerializeField] private Image cooldownOverlay;

    // CACHE
    private ActionStore _store;
    private CooldownStore _cooldownStore;

    // LIFECYCLE METHODS
    private void Awake()
    {
      var player = GameObject.FindGameObjectWithTag(Tag.Player);

      _store = player.GetComponent<ActionStore>();
      _cooldownStore = player.GetComponent<CooldownStore>();


      _store.StoreUpdated += UpdateIcon;
    }

    private void Update()
    {
      cooldownOverlay.fillAmount = _cooldownStore.GetCooldownFractionRemaining(GetItem());
    }

    // PUBLIC

    public void AddItems(InventoryItem item, int number)
    {
      _store.AddAction(item, index, number);
    }

    public InventoryItem GetItem()
    {
      return _store.GetAction(index);
    }

    public int GetNumber()
    {
      return _store.GetNumber(index);
    }

    public int MaxAcceptable(InventoryItem item)
    {
      return _store.MaxAcceptable(item, index);
    }

    public void RemoveItems(int number)
    {
      _store.RemoveItems(index, number);
    }

    // PRIVATE

    void UpdateIcon()
    {
      icon.SetItem(GetItem(), GetNumber());
    }
  }
}