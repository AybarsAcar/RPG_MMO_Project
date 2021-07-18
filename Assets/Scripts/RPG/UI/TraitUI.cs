using System;
using RPG.Core.Util;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
  public class TraitUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI unassignedPointsText;
    [SerializeField] private Button commitButton;

    private TraitStore _playerTraitStore;

    private void Start()
    {
      _playerTraitStore = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<TraitStore>();

      commitButton.onClick.AddListener(() => { _playerTraitStore.Commit(); });
    }

    private void Update()
    {
      unassignedPointsText.text = _playerTraitStore.GetUnassignedPoints().ToString();
    }
  }
}