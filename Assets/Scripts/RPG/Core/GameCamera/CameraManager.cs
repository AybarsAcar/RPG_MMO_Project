using RPG.Core.Util;
using UnityEngine;

namespace RPG.Core.GameCamera
{
  public class CameraManager : MonoBehaviour
  {
    [SerializeField] private Vector2 panLimit = new Vector2(40f, 40f);
    
    private Transform _playerTransform;

    private Vector3 _cameraOffset;
    private Vector3 _cameraRotation;

    private const float SmoothSpeed = 1f;
    private const float EdgeSize = 10f; // how close the cursor needs to get to the edge of the screen to scroll 
    private const float PanSpeed = 20f; // move speed of the camera when scrolling
    private const float ZoomSpeed = 2f;

    private float _minY = 20f;
    private float _maxY = 120f;

    private bool _isCameraLockedOnPlayer;

    private void Awake()
    {
      var player = GameObject.FindWithTag(Tag.Player);
      _playerTransform = player.transform;
      
      // set the camera offset and rotation
      _cameraOffset = new Vector3(0, 20, -10);
      _cameraRotation = new Vector3(60, 0, 0);
    }

    private void Start()
    {
      // set rotation
      transform.rotation = Quaternion.Euler(_cameraRotation);
      transform.position = _playerTransform.position + _cameraOffset;
    }


    private void FixedUpdate()
    {
      ToggleCameraLockOnPlayer();

      HandleZoom();

      if (_isCameraLockedOnPlayer)
      {
        HandleMovement();
      }

      if (!_isCameraLockedOnPlayer)
      {
        HandleEdgeScrolling();
      }
    }


    private void HandleMovement()
    {
      var cameraFollowPosition = _playerTransform.position;

      var desiredPosition = cameraFollowPosition + _cameraOffset;
      var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, (SmoothSpeed * Time.deltaTime));

      transform.position = smoothedPosition;
    }

    /**
   * TODO: zoom requires fixes for follow camera
   */
    private void HandleZoom()
    {
      var cameraPosition = transform.position;

      var scroll = Input.GetAxis("Mouse ScrollWheel");

      cameraPosition.y -= scroll * 100 * ZoomSpeed * Time.deltaTime;

      transform.position = cameraPosition;
    }

    private void HandleEdgeScrolling()
    {
      var cameraPosition = transform.position;


      if (Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - EdgeSize)
      {
        cameraPosition.z += PanSpeed * Time.deltaTime;
      }

      if (Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= EdgeSize)
      {
        cameraPosition.z -= PanSpeed * Time.deltaTime;
      }

      if (Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - EdgeSize)
      {
        cameraPosition.x += PanSpeed * Time.deltaTime;
      }

      if (Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= EdgeSize)
      {
        cameraPosition.x -= PanSpeed * Time.deltaTime;
      }

      cameraPosition.x = Mathf.Clamp(cameraPosition.x, -panLimit.x, panLimit.x);
      cameraPosition.z = Mathf.Clamp(cameraPosition.z, -panLimit.y, panLimit.y);

      transform.position = cameraPosition;
    }

    private void ToggleCameraLockOnPlayer()
    {
      if (Input.GetKeyDown(KeyCode.Y))
      {
        _isCameraLockedOnPlayer = !_isCameraLockedOnPlayer;
      }
    }
  }
}