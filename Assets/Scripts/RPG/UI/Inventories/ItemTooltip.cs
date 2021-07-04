using RPG.Inventories;
using RPG.Utils.UI.Dragging.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventories
{
  /// <summary>
  /// Root of the tooltip prefab to expose properties to other classes.
  /// </summary>
  public class ItemTooltip : MonoBehaviour
  {
    // CONFIG DATA
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI bodyText = null;

    // PUBLIC

    public void Setup(InventoryItem item)
    {
      titleText.text = item.DisplayName;
      bodyText.text = item.Description;
    }
  }
}