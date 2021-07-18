using RPG.Combat;
using RPG.Core;
using RPG.Core.Util;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
  public class AIController : MonoBehaviour
  {
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private PatrolPath patrolPath;
    [SerializeField] private float waypointTolerance = 1f;

    [SerializeField] [Range(0, 1)] private float patrolSpeedFraction = 0.2f;

    [SerializeField] private float shoutDistance = 5f; // the distance that they let nearby enemies know

    private Fighter _fighter;
    private Health _health;
    private Mover _mover;
    private ActionScheduler _actionScheduler;

    private GameObject _player;

    // states
    // cache the initial position
    private Vector3 _guardPosition;
    private float _timeSinceLastSawPlayer = Mathf.Infinity;
    private const float SuspicionStateDuration = 5f;
    private float _timeSinceAggravated = Mathf.Infinity;
    private const float AggroCooldownTime = 5f;

    private int _currentWaypointIndex = 0;
    private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
    private readonly float _dwellTime = 3f;

    private void Awake()
    {
      _fighter = GetComponent<Fighter>();
      _health = GetComponent<Health>();
      _mover = GetComponent<Mover>();
      _actionScheduler = GetComponent<ActionScheduler>();

      _player = GameObject.FindWithTag(Tag.Player);
    }

    private void Start()
    {
      // set the initial state
      _guardPosition = transform.position;
    }

    private void Update()
    {
      if (_health.IsDead) return;

      if (IsAggravated() && _fighter.CanAttack(_player))
      {
        AttackBehaviour();
      }
      else if (_timeSinceLastSawPlayer <= SuspicionStateDuration)
      {
        SuspicionBehaviour();
      }
      else
      {
        PatrolBehaviour();
      }

      _timeSinceLastSawPlayer += Time.deltaTime;
      _timeSinceArrivedAtWaypoint += Time.deltaTime;
      _timeSinceAggravated += Time.deltaTime;
    }

    public void Aggravate()
    {
      // sets the timer
      _timeSinceAggravated = 0;
    }

    private bool IsAggravated()
    {
      var distance = Vector3.Distance(_player.transform.position, transform.position);
      return distance <= chaseDistance || _timeSinceAggravated < AggroCooldownTime;
    }

    private void PatrolBehaviour()
    {
      var nextPosition = _guardPosition;

      if (patrolPath != null)
      {
        // patrol
        if (AtWaypoint())
        {
          _timeSinceArrivedAtWaypoint = 0;
          CycleWaypoint();
        }

        nextPosition = GetCurrentWaypoint();
      }

      if (_timeSinceArrivedAtWaypoint > _dwellTime)
      {
        // cancel attack if the player runs out of the chaseDistance - StartMoveAction cancels the attack in it
        // and goes back to the guard position
        _mover.StartMoveAction(nextPosition, patrolSpeedFraction);
      }
    }

    private Vector3 GetCurrentWaypoint()
    {
      return patrolPath.GetWaypoint(_currentWaypointIndex);
    }

    private void CycleWaypoint()
    {
      _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
    }

    private bool AtWaypoint()
    {
      var distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());

      return distanceToWaypoint <= waypointTolerance;
    }

    private void AttackBehaviour()
    {
      _timeSinceLastSawPlayer = 0; // reset the time
      _fighter.Attack(_player);

      AggravateNearbyEnemies();
    }

    /// <summary>
    /// Sphere cast is implemented
    /// our sphere cast will be static around our enemies
    /// so no direction
    /// Aggravates enemies within the sphere cast
    /// </summary>
    private void AggravateNearbyEnemies()
    {
      var hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0f);

      foreach (var hit in hits)
      {
        // find enemies
        var enemy = hit.collider.GetComponent<AIController>();

        // call aggravate on nearby enemies
        if (enemy != null)
        {
          enemy.Aggravate();
        }
      }
    }

    private void SuspicionBehaviour()
    {
      // Suspicion state
      _actionScheduler.CancelCurrentAction();
    }

    /// <summary>
    /// called by Unity
    /// draws a gizmos when the gameObject with this script is selected
    /// </summary>
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

    /// <summary>
    /// resets the enemy AI
    /// called when player respawns
    /// </summary>
    public void Reset()
    {
      // warp back to the start
      GetComponent<NavMeshAgent>().Warp(_guardPosition);

      // reset the other states
      _timeSinceLastSawPlayer = Mathf.Infinity;
      _timeSinceArrivedAtWaypoint = Mathf.Infinity;
      _timeSinceAggravated = Mathf.Infinity;
      _currentWaypointIndex = 0;
    }
  }
}