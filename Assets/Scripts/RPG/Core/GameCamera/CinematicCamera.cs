using UnityEngine;

namespace RPG.Core.GameCamera
{
  public class CinematicCamera : MonoBehaviour
  {
    [SerializeField] private Transform enemy; // camera to follow

    private void Update()
    {
      transform.LookAt(enemy);
    }
  }
}