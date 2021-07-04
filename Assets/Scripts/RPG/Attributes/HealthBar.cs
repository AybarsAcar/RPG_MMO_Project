using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Attributes
{
  public class HealthBar : MonoBehaviour
  {
    [SerializeField] private RectTransform foregroundImage;
    [SerializeField] private Health health;

    private void Update()
    {
      foregroundImage.localScale = new Vector3(health.GetNormalizedHealth(), 1f, 1f);

      if (health.IsDead)
      {
        Destroy(gameObject);
      }
    }
  }
}