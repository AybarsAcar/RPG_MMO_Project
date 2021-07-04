using System;
using UnityEngine;

namespace RPG.Core
{
  /**
   * Used the Rotate the canvas that has the health bar
   * damage point indicators
   * to always face the Camera
   */
  public class CameraFacing : MonoBehaviour
  {
    private Camera _camera;

    private void Awake()
    {
      _camera = Camera.main;
    }

    private void LateUpdate()
    {
      transform.forward = _camera.transform.forward;
    }
  }
}