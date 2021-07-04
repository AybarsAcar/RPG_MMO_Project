using UnityEngine;

namespace RPG.Core
{
  public class PatrolPath : MonoBehaviour
  {
    private const float WaypointGizmosRadius = 0.3f;

    /**
     * to connect the Waypoints
     * and draw waypoints
     */
    private void OnDrawGizmos()
    {
      // iterate over the PatrolPath gameObjects children
      // which are waypoints
      for (int i = 0; i < transform.childCount; i++)
      {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetWaypoint(i), WaypointGizmosRadius);
        
        Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i)));
      }
    }

    public Vector3 GetWaypoint(int i)
    {
      return transform.GetChild(i).position;
    }

    public int GetNextIndex(int i)
    {
      if (i + 1 == transform.childCount)
      {
        return 0;
      }

      return i + 1;
    }
  }
}