using System;
using UnityEngine;

namespace RPG.UI
{
  /// <summary>
  /// UI Switcher, attached to the Switcher Game Object in the UI
  /// used to conditionally render its children in UI
  /// </summary>
  public class SwitcherUI : MonoBehaviour
  {
    [SerializeField] private GameObject entryPoint;

    private void Start()
    {
      SwitchTo(entryPoint);
    }

    public void SwitchTo(GameObject uiToDisplay)
    {
      if (uiToDisplay.transform.parent != transform)
      {
        // if passed in ui is not our child
        return;
      }

      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(child.gameObject == uiToDisplay);
      }
    }
  }
}