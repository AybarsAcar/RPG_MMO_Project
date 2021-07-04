using System;
using System.Globalization;
using RPG.Core.Util;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
  public class HealthDisplay : MonoBehaviour
  {
    private Text _healthDisplay;
    private Health _health;

    private void Awake()
    {
      _healthDisplay = GetComponent<Text>();
      _health = GameObject.FindWithTag(Tag.Player).GetComponent<Health>();
    }

    private void Update()
    {
      // :0 makes 0 decimal points, 0.0 is 1 decimal place
      _healthDisplay.text = $"{_health.HealthPoints:0} / {_health.GetMaxHealthPoints():0}"; 
    }
  }
}