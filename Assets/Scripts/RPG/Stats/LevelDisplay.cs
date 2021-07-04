using RPG.Core.Util;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
  public class LevelDisplay : MonoBehaviour
  {
    private Text _levelDisplay;
    private BaseStats _stats;
    
    private void Awake()
    {
      _levelDisplay = GetComponent<Text>();
      _stats = GameObject.FindWithTag(Tag.Player).GetComponent<BaseStats>();
    }

    private void Update()
    {
      _levelDisplay.text = _stats.GetPlayerLevel().ToString();
    }
  }
}