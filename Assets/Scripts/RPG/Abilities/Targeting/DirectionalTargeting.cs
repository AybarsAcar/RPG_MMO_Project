using System;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
  [CreateAssetMenu(fileName = "New Directional Targeting",
    menuName = "Abilities/Targeting Strategy/New Directional Targeting")]
  public class DirectionalTargeting : TargetingStrategy
  {
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float groundOffset = 1f;

    public override void StartTargeting(AbilityData data, Action onFinish)
    {
      var ray = PlayerController.GetMouseRay();
      // if raycast successfully hits anything
      if (Physics.Raycast(ray, out var raycastHit, 1000, layerMask))
      {
        // to avoid ability going into the terrain
        // data.TargetedPoint = raycastHit.point + ray.direction * groundOffset / ray.direction.y;
        data.TargetedPoint = new Vector3(raycastHit.point.x, groundOffset, raycastHit.point.z);
      }

      onFinish();
    }
  }
}