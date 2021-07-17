using RPG.Core.Util;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
  public class ManaDisplay : MonoBehaviour
  {
    private Text _manaDisplay;
    private Mana _mana;

    private void Awake()
    {
      _manaDisplay = GetComponent<Text>();
      _mana = GameObject.FindWithTag(Tag.Player).GetComponent<Mana>();
    }

    private void Update()
    {
      // :0 makes 0 decimal points, 0.0 is 1 decimal place
      _manaDisplay.text = $"{_mana.CurrentMana:0} / {_mana.MaxMana:0}"; 
    }
  }
}