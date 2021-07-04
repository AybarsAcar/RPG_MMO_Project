using RPG.Core.Util;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
  public class ExperienceDisplay : MonoBehaviour
  {
    private Text _experienceDisplay;
    private Experience _experience;
    
    private void Awake()
    {
      _experienceDisplay = GetComponent<Text>();
      _experience = GameObject.FindWithTag(Tag.Player).GetComponent<Experience>();
    }

    private void Update()
    {
      _experienceDisplay.text = _experience.ExperiencePoints.ToString();
    }
  }
}