using System;
using System.Globalization;
using RPG.Attributes;
using RPG.Core.Util;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
  public class EnemyHealthDisplay : MonoBehaviour
  {
    private Text _healthDisplay;
    private Fighter _playerFighter;

    private void Awake()
    {
      _healthDisplay = GetComponent<Text>();
      _playerFighter = GameObject.FindWithTag(Tag.Player).GetComponent<Fighter>();

    }

    private void Update()
    {
      var health = _playerFighter.Target;

      _healthDisplay.text = health == null ? "N/A" : $"{health.HealthPoints:0} / {health.GetMaxHealthPoints():0}";
    }
  }
}