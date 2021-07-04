using JetBrains.Annotations;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
  public class Mover : MonoBehaviour, IAction, ISavable
  {
    [SerializeField] private float maxSpeed = 6f;

    [SerializeField] private float maxNavPathLenght = 40f;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Health _health;

    private ActionScheduler _actionScheduler;


    private void Awake()
    {
      _navMeshAgent = GetComponent<NavMeshAgent>();
      _animator = GetComponent<Animator>();
      _health = GetComponent<Health>();

      _actionScheduler = GetComponent<ActionScheduler>();
    }

    private void Update()
    {
      // disable the navmesh agent if the character is dead
      // so player can move over it
      _navMeshAgent.enabled = !_health.IsDead;

      UpdateAnimator();
    }


    private void UpdateAnimator()
    {
      // get the global velocity
      var globalVelocity = _navMeshAgent.velocity;

      // convert into a local variable
      var localVelocity = transform.InverseTransformDirection(globalVelocity);

      var speed = localVelocity.z;

      _animator.SetFloat("ForwardSpeed", speed);
    }

    public void StartMoveAction(Vector3 destination, float speedFraction)
    {
      _actionScheduler.StartAction(this);

      MoveTo(destination, speedFraction);
    }

    public bool CanMoveTo(Vector3 destination)
    {
      var path = new NavMeshPath();
      var hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

      if (!hasPath) return false;
      
      // when there is no path available
      if (path.status != NavMeshPathStatus.PathComplete) return false;

      // return false if the path is too long
      if (GetPathLength(path) > maxNavPathLenght) return false;

      return true;
    }

    private float GetPathLength(NavMeshPath path)
    {
      var totalDistance = 0f;

      if (path.corners.Length < 2)
      {
        return totalDistance;
      }

      for (int i = 1; i < path.corners.Length; i++)
      {
        totalDistance += Vector3.Distance(path.corners[i], path.corners[i - 1]);
      }

      return totalDistance;
    }

    public void MoveTo(Vector3 destination, float speedFraction)
    {
      _navMeshAgent.SetDestination(destination);
      _navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
      _navMeshAgent.isStopped = false;
    }

    public void Cancel()
    {
      _navMeshAgent.isStopped = true;
    }

    public object CaptureState()
    {
      return new SerializableVector3(transform.position);
    }

    public void RestoreState(object state)
    {
      var position = ((SerializableVector3) state).ToVector3();

      _navMeshAgent.enabled = false;

      transform.position = position;

      _navMeshAgent.enabled = true;

      _actionScheduler.CancelCurrentAction();
    }
  }
}