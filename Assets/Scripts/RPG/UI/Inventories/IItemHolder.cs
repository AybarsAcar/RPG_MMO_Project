using RPG.Inventories;
using RPG.Utils.UI.Dragging.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Allows the `ItemTooltipSpawner` to display the right information.
    /// </summary>
    public interface IItemHolder
    {
        InventoryItem GetItem();
    }
}