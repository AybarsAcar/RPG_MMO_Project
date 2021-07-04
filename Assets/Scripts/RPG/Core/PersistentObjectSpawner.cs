using UnityEngine;

namespace RPG.Core
{
  public class PersistentObjectSpawner : MonoBehaviour
  {
    [SerializeField] private GameObject persistentObjectPrefab;

    static bool HasSpawned;

    private void Awake()
    {
      if (HasSpawned) return;

      SpawnPersistentObjects();

      HasSpawned = true;
    }

    private void SpawnPersistentObjects()
    {
      var persistentGameObject = Instantiate(persistentObjectPrefab);
      DontDestroyOnLoad(persistentGameObject);
    }
  }
}