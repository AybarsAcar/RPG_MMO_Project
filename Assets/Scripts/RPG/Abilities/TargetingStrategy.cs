using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
  public abstract class TargetingStrategy : ScriptableObject
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user">The game object using the ability - Player</param>
    /// <param name="onFinish">method callback called when the targeting finishes</param>
    public abstract void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> onFinish);
  }
}