using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
  public abstract class FilterStrategy : ScriptableObject
  {
    /// <summary>
    /// filter the game objets that the raycast hit
    /// depending on the ability feature
    /// </summary>
    /// <param name="objectsToFilter"></param>
    /// <returns></returns>
    public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter);
  }
}