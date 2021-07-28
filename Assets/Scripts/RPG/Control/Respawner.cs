using System.Collections;
using RPG.Attributes;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
  /// <summary>
  /// Responsible for respawning the player when the player dies
  /// attached to the Player Game Object
  /// </summary>
  public class Respawner : MonoBehaviour
  {
    [SerializeField] private Transform respawnLocation;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private float fadeTime = 0.2f;
    [SerializeField] private float healthRegenPercentage = 0.6f;
    [SerializeField] private float enemyHealthRegenRate = 0.8f;


    private Health _health;

    private void Awake()
    {
      _health = GetComponent<Health>();
      _health.onDie.AddListener(Respawn);
    }

    private void Start()
    {
      if (_health.IsDead) Respawn();
    }

    private void Respawn()
    {
      StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
      // save the game so the player won't quit immediately as they die
      FindObjectOfType<SavingWrapper>().SaveGameState();

      yield return new WaitForSeconds(respawnDelay);

      var fader = FindObjectOfType<Fader>();

      yield return fader.FadeOut(fadeTime);

      // set the location to start location
      GetComponent<NavMeshAgent>().Warp(respawnLocation.position);

      _health.Heal(_health.GetMaxHealthPoints() * healthRegenPercentage);

      ResetEnemies();

      yield return fader.FadeIn(fadeTime);
    }

    private void ResetEnemies()
    {
      foreach (var enemyController in FindObjectsOfType<AIController>())
      {
        var enemyHealth = enemyController.GetComponent<Health>();

        if (enemyHealth && !enemyHealth.IsDead)
        {
          // so the enemies stop chasing after respawn
          enemyController.Reset();

          // regen some health
          _health.Heal(_health.GetMaxHealthPoints() * enemyHealthRegenRate);
        }
      }
    }
  }
}