using System;
using RPG.Control;
using RPG.Core;
using RPG.Core.Util;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematic
{
  /**
   * disables player controls when cinematic plays
   */
  public class CinematicControlRemover : MonoBehaviour
  {
    private GameObject _player;

    private void Awake()
    {
      _player = GameObject.FindGameObjectWithTag(Tag.Player);
    }

    private void OnEnable()
    {
      // add events to listen to
      GetComponent<PlayableDirector>().played += DisableControl;
      GetComponent<PlayableDirector>().stopped += EnableControl;
    }

    private void OnDisable()
    {
      // add events to listen to
      GetComponent<PlayableDirector>().played -= DisableControl;
      GetComponent<PlayableDirector>().stopped -= EnableControl;
    }

    private void DisableControl(PlayableDirector playableDirector)
    {
      _player.GetComponent<ActionScheduler>().CancelCurrentAction(); // cancel current action, stop player
      _player.GetComponent<PlayerController>().enabled = false; // disable player controls
    }

    private void EnableControl(PlayableDirector playableDirector)
    {
      _player.GetComponent<PlayerController>().enabled = true;
    }
  }
}