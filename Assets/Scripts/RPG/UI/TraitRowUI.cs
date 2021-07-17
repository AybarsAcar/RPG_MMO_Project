using RPG.Core.Util;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
  public class TraitRowUI : MonoBehaviour
  {
    [SerializeField] private Trait trait;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;

    private TraitStore _playerTraitStore;

    private void Start()
    {
      _playerTraitStore = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<TraitStore>();

      minusButton.onClick.AddListener(() => Allocate(-1));
      plusButton.onClick.AddListener(() => Allocate(1));
    }

    private void Update()
    {
      minusButton.interactable = _playerTraitStore.CanAssignPoints(trait, -1);
      plusButton.interactable = _playerTraitStore.CanAssignPoints(trait, +1);

      valueText.text = _playerTraitStore.GetPoints(trait).ToString();
    }

    /// <summary>
    /// minus / plus button callback
    /// </summary>
    /// <param name="points"></param>
    private void Allocate(int points)
    {
      _playerTraitStore.AssignPoints(trait, points);
    }
  }
}