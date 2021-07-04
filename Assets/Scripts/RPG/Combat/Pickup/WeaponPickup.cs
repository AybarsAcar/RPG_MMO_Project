using System;
using System.Collections;
using RPG.Attributes;
using RPG.Control;
using RPG.Core.Util;
using RPG.Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Combat.Pickup
{
  public class WeaponPickup : MonoBehaviour, IRaycastable
  {
    [SerializeField] private WeaponConfig weaponConfig = null;

    [SerializeField] private float healthToRestore = 0f;

    private const float RespawnTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
      if (!other.CompareTag(Tag.Player)) return;

      Pickup(other.gameObject);
    }

    private void Pickup(GameObject subject)
    {
      if (weaponConfig != null)
      {
        subject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
      }

      if (healthToRestore > 0)
      {
        subject.GetComponent<Health>().Heal(healthToRestore);
      }

      StartCoroutine(HideForSeconds(RespawnTime));
    }

    private IEnumerator HideForSeconds(float second)
    {
      // disable game object
      ShowPickup(false);

      yield return new WaitForSeconds(5f);

      // enable game object
      ShowPickup(true);
    }

    private void ShowPickup(bool isShowing)
    {
      GetComponent<Collider>().enabled = isShowing;

      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(isShowing);
      }
    }

    public bool HandleRaycast(PlayerController callingController)
    {
      if (Input.GetMouseButtonDown(1))
      {
        Pickup(callingController.gameObject);
      }

      return true;
    }

    public CursorType GetCursorType()
    {
      return CursorType.Pickup;
    }
  }
}