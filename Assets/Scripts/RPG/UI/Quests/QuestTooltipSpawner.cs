using RPG.Utils.UI.Tooltips;
using UnityEngine;

namespace RPG.UI.Quests
{
  public class QuestTooltipSpawner : TooltipSpawner
  {
    public override void UpdateTooltip(GameObject tooltip)
    {
      var status = GetComponent<QuestItemUI>().Status;
      tooltip.GetComponent<QuestTooltipUI>().Setup(status);
    }

    public override bool CanCreateTooltip()
    {
      return true;
    }
  }
}