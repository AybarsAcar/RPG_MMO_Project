using System.Collections;
using System.Linq;
using RPG.Control;
using RPG.Core.Util;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
  public class Portal : MonoBehaviour
  {
    // make sure to make the destinations in different scenes matching
    // A in one scene will transport to A in another scene
    enum DestinationIdentifier
    {
      A,
      B,
      C,
      D
    }


    [SerializeField] private DestinationIdentifier destination;
    [SerializeField] private int loadSceneIndex;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeWaitTime = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
      if (!other.CompareTag(Tag.Player)) return;

      StartCoroutine(TransitionToScene(loadSceneIndex));
    }

    private IEnumerator TransitionToScene(int sceneToLoadIndex)
    {
      DontDestroyOnLoad(gameObject);

      var fader = FindObjectOfType<Fader>();
      var saver = FindObjectOfType<SavingWrapper>();

      // remove player control
      var playerController
        = GameObject.FindWithTag(Tag.Player).GetComponent<PlayerController>();
      playerController.enabled = false;

      yield return fader.FadeOut(fadeOutTime);

      saver.SaveGameState();

      yield return SceneManager.LoadSceneAsync(sceneToLoadIndex);

      // get the player object again from the new scene
      var playerControllerNewScene
        = GameObject.FindWithTag(Tag.Player).GetComponent<PlayerController>();

      // remove player control
      playerControllerNewScene.enabled = false;

      saver.LoadGameState();

      yield return new WaitForEndOfFrame();

      var otherPortal = GetOtherPortal();
      UpdatePlayer(otherPortal);

      saver.SaveGameState();

      yield return new WaitForSeconds(fadeWaitTime);

      fader.FadeIn(fadeInTime);

      // restore the player controls
      playerControllerNewScene.enabled = true;

      Destroy(gameObject);
    }

    private void UpdatePlayer(Portal otherPortal)
    {
      var player = GameObject.FindWithTag(Tag.Player);

      player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position); // to avoid conflict

      player.transform.position = otherPortal.spawnPoint.position;
      player.transform.rotation = otherPortal.spawnPoint.rotation;
    }

    private Portal GetOtherPortal()
    {
      var portals = FindObjectsOfType<Portal>();

      return portals
        .Where(portal => portal != this)
        .FirstOrDefault(portal => portal.destination == destination);
    }
  }
}