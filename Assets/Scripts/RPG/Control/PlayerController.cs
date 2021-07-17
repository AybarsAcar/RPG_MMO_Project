using System;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
  public class PlayerController : MonoBehaviour
  {
    [Serializable]
    private struct CursorMapping
    {
      public CursorType type;
      public Texture2D texture;
      public Vector2 hotspot;
    }

    [SerializeField] private CursorMapping[] cursorMappings;

    [SerializeField] private float maxNavMeshProjectionDistance = 1f;

    [SerializeField] private int numberOfAbilities = 6;

    private Mover _mover;
    private ActionScheduler _actionScheduler;
    private Health _health;
    private ActionStore _actionStore;

    private bool _isDraggingUI; // is updated when interacting with the UI

    private void Awake()
    {
      _mover = GetComponent<Mover>();
      _actionScheduler = GetComponent<ActionScheduler>();
      _health = GetComponent<Health>();
      _actionStore = GetComponent<ActionStore>();
    }

    void Update()
    {
      if (InteractWithUI()) return;


      if (_health.IsDead)
      {
        SetCursor(CursorType.None);
        return;
      }

      // we can use ability and interact with movement concurrently
      // so no return value
      UseAbilities();

      if (Input.GetKeyDown(KeyCode.S))
      {
        StopAllActions();
        return;
      }

      if (InteractWithComponent()) return;

      if (HandlePlayerMovement()) return;

      SetCursor(CursorType.None);
    }

    /// <summary>
    /// calls an ability from our ActionStore
    /// KeyCode enum is sequential
    /// Alpha 1 starts at 49
    /// </summary>
    private void UseAbilities()
    {
      // loop over the buttons
      for (int i = 0; i < numberOfAbilities; i++)
      {
        if (Input.GetKeyDown(KeyCode.Alpha1 + i))
        {
          _actionStore.Use(i, gameObject);
        }
      }
    }

    /**
     * raycast through the world getting all the hits
     */
    private bool InteractWithComponent()
    {
      var hits = RaycastHitsAllSorted();
      foreach (var raycastHit in hits)
      {
        var raycastables = raycastHit.transform.GetComponents<IRaycastable>();
        foreach (var raycastable in raycastables)
        {
          if (raycastable.HandleRaycast(this))
          {
            SetCursor(raycastable.GetCursorType());
            return true;
          }
        }
      }

      return false;
    }


    /**
     * returns the raycast hits sorted based on their depth
     * the object closer to the camera will be before the one behind
     */
    private RaycastHit[] RaycastHitsAllSorted()
    {
      // get all the hits
      var hits = Physics.SphereCastAll(GetMouseRay(), 1f); // so its easier to pick

      // sort by depth distance
      // build array of distances
      var distances = new float[hits.Length];

      for (int i = 0; i < hits.Length; i++)
      {
        distances[i] = hits[i].distance;
      }

      // sort hits
      Array.Sort(distances, hits); // rearranges the hits array in place

      return hits;
    }

    private bool InteractWithUI()
    {
      if (Input.GetMouseButtonUp(0))
      {
        _isDraggingUI = false;
      }

      if (!EventSystem.current.IsPointerOverGameObject()) return false;

      if (Input.GetMouseButtonDown(0))
      {
        _isDraggingUI = true;
      }

      if (_isDraggingUI)
      {
        return true;
      }

      SetCursor(CursorType.UI);
      return true;
    }

    private void StopAllActions()
    {
      _actionScheduler.CancelCurrentAction();
    }

    /**
    * moves the player to where the mouse is clicked
    */
    private bool HandlePlayerMovement()
    {
      // out stores the position the position of the raycast hit

      var hasHit = RaycastNavMesh(out var target);

      if (!hasHit) return false;

      if (!_mover.CanMoveTo(target)) return false;

      if (Input.GetMouseButton(1))
      {
        // if hit is true set the point of collision the destination
        _mover.StartMoveAction(target, 1f);
      }

      SetCursor(CursorType.Movement);
      return true;
    }

    /**
     * out assigns a target and outs it
     * it is pass by reference,
     * we can declare it while passing
     * target - is the target we want to move to
     *
     * if will return true if the navmesh point available - which means walkable
     * finds the closest point
     */
    private bool RaycastNavMesh(out Vector3 target)
    {
      target = new Vector3();

      // raycast to terrain
      var hasHit = Physics.Raycast(GetMouseRay(), out var hit);

      if (!hasHit) return false;

      // find the nearest navmesh point
      var hasCastToNavmesh =
        NavMesh.SamplePosition(hit.point, out var navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

      if (!hasCastToNavmesh) return false;

      // set the target before returning
      target = navMeshHit.position;


      // return true if exists - which means its walkable
      return true;
    }


    public static Ray GetMouseRay()
    {
      return Camera.main.ScreenPointToRay(Input.mousePosition);
    }


    private void SetCursor(CursorType type)
    {
      var mapping = GetCursorMapping(type);
      Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
    }

    private CursorMapping GetCursorMapping(CursorType type)
    {
      foreach (var cursorMapping in cursorMappings)
      {
        if (cursorMapping.type == type)
        {
          return cursorMapping;
        }
      }

      return cursorMappings[0];
    }
  }
}