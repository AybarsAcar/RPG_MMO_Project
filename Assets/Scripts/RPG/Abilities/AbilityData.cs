using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
  public class AbilityData : IAction
  {
    public GameObject User { get; set; }
    public IEnumerable<GameObject> Targets { get; set; }
    public Vector3 TargetedPoint { get; set; }

    private bool _isCancelled = false;
    public bool IsCancelled => _isCancelled;

    /// <summary>
    /// handles the coroutine on any object on the User
    /// </summary>
    /// <param name="coroutine"></param>
    public void StartCoroutine(IEnumerator coroutine)
    {
      User.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Cancel()
    {
      _isCancelled = true;
    }
  }
}