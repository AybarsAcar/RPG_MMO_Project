using System;
using UnityEngine;

namespace RPG.Abilities
{
  public abstract class EffectStrategy : ScriptableObject
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onFinish"></param>
    public abstract void StartEffect(AbilityData data, Action onFinish);
  }
}