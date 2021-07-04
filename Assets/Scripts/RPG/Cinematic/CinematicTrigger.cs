using RPG.Core.Util;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematic
{
  public class CinematicTrigger : MonoBehaviour
  {
    private bool _isTriggered = false;


    private void OnTriggerEnter(Collider other)
    {
      if (_isTriggered || !other.gameObject.CompareTag(Tag.Player)) return;

      GetComponent<PlayableDirector>().Play();

      _isTriggered = true;
    }
  }
}