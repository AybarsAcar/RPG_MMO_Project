using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
  /// <summary>
  /// this will be click have a cursor show up and then click to execute the ability
  /// </summary>
  [CreateAssetMenu(fileName = "New Delayed Click Targeting",
    menuName = "Abilities/Targeting Strategy/New Delayed Click Targeting")]
  public class DelayedClickTargeting : TargetingStrategy
  {
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float areaOfEffectRadius;
    [SerializeField] private Transform targetingPrefab;

    private Transform _targetingIndicator;

    public override void StartTargeting(AbilityData data, Action onFinish)
    {
      var playerController = data.User.GetComponent<PlayerController>();

      playerController.StartCoroutine(Targeting(data, playerController, onFinish));
    }

    private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action onFinish)
    {
      playerController.enabled = false;

      // instantiate the targeting indicator
      if (_targetingIndicator == null)
      {
        _targetingIndicator = Instantiate(targetingPrefab);
      }
      else
      {
        // or set it to active if already instantiated
        _targetingIndicator.gameObject.SetActive(true);
      }

      // set its size to the size of the sphere raycast radius
      _targetingIndicator.localScale = new Vector3(areaOfEffectRadius * 2, 1, areaOfEffectRadius * 2);

      // we need a while loop that runs every frame if the player is not dead 
      while (!data.IsCancelled)
      {
        // Runs every frame
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);

        // if raycast successfully hits anything
        if (Physics.Raycast(PlayerController.GetMouseRay(), out var raycastHit, 1000, layerMask))
        {
          // update the position of the targeting indicator
          _targetingIndicator.position = raycastHit.point;

          if (Input.GetMouseButtonDown(0))
          {
            // so we wait for the actual click when button is lifted
            yield return new WaitWhile(() => Input.GetMouseButton(0));

            // set the mouse click position
            data.TargetedPoint = raycastHit.point;

            // set the targets
            data.Targets = GetGameObjectsInRadius(raycastHit.point);

            break;
          }
        }

        yield return null;
      }

      _targetingIndicator.gameObject.SetActive(false);
      playerController.enabled = true;
      onFinish();
    }

    private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
    {
      // apply the sphere cast
      var hits = Physics.SphereCastAll(point, areaOfEffectRadius, Vector3.up, 0);

      foreach (var hit in hits)
      {
        yield return hit.collider.gameObject;
      }
    }
  }
}