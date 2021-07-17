using System;
using UnityEngine;

namespace RPG.Abilities
{
  public abstract class TargetingStrategy : ScriptableObject
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">The Ability Data includes The game object using the ability - Player</param>
    /// <param name="onFinish">method callback called when the targeting finishes</param>
    public abstract void StartTargeting(AbilityData data, Action onFinish);
  }
}