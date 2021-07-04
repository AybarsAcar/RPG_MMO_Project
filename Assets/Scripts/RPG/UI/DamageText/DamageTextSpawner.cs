using System;
using UnityEngine;

namespace RPG.UI.DamageText
{
  public class DamageTextSpawner : MonoBehaviour
  {
    [SerializeField] private DamageText damageTextPrefab;
    

    public void Spawn(float damage)
    {
      var instance = Instantiate(damageTextPrefab, transform);
      instance.SetValue(damage);
    }
  }
}