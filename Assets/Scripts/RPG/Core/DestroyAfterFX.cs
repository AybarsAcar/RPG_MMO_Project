using UnityEngine;

namespace RPG.Core
{
  public class DestroyAfterFX : MonoBehaviour
  {
    [SerializeField] private GameObject targetToDestroy = null;

    private void Update()
    {
      if (GetComponent<ParticleSystem>().IsAlive()) return;
      
      Destroy(targetToDestroy != null ? targetToDestroy : gameObject);
    }
  }
}