using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
  public class AbilityData
  {
    public GameObject User { get; set; }
    public IEnumerable<GameObject> Targets { get; set; }
    public Vector3 TargetedPoint { get; set; }

    /// <summary>
    /// handles the coroutine on any object on the User
    /// </summary>
    /// <param name="coroutine"></param>
    public void StartCoroutine(IEnumerator coroutine)
    {
      User.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
    }
  }
}