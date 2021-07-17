using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
  public class Projectile : MonoBehaviour
  {
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool isHoming = true; // if true, tracks the character movement
    [SerializeField] private GameObject hitFX = null;
    [SerializeField] private GameObject[] destroyOnHit = null;

    [SerializeField] private UnityEvent onHitSFX;

    private Health _target;
    private float _damage = 0f; // is passed in from the Weapon.cs 
    private GameObject _instigator;
    private Vector3 _targetPoint;

    private const float MaxLifeTime = 15f;
    private const float LifeAfterImpact = 0.1f;

    private void Start()
    {
      transform.LookAt(GetAimLocation());
    }

    private void Update()
    {
      if (_target != null && isHoming && !_target.IsDead)
      {
        transform.LookAt(GetAimLocation());
      }

      transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
      SetTarget(instigator, damage, target);
    }

    public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
    {
      SetTarget(instigator, damage, null, targetPoint);
    }

    private void SetTarget(GameObject instigator, float damage, Health target = null, Vector3 targetPoint = default)
    {
      _target = target;

      _targetPoint = targetPoint;

      _damage = damage;
      _instigator = instigator;

      Destroy(gameObject, MaxLifeTime);
    }

    /// <summary>
    /// returns the body chest of the Character that targets
    /// we use capsule colliders height
    /// because our collider is the height of our player
    /// </summary>
    /// <returns></returns>
    private Vector3 GetAimLocation()
    {
      if (_target == null)
      {
        return _targetPoint;
      }

      var targetCapsule = _target.GetComponent<CapsuleCollider>();

      if (targetCapsule == null) return _target.transform.position;

      return _target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

    /// <summary>
    /// when the projectile collides with the enemy
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
      var otherHealth = other.GetComponent<Health>();

      if (_target != null && otherHealth != _target) return;

      if (otherHealth == null || otherHealth.IsDead) return;

      // colliding with ourselves
      if (other.gameObject == _instigator) return;

      otherHealth.TakeDamage(_instigator, _damage);

      speed = 0;

      onHitSFX.Invoke();

      if (hitFX != null)
      {
        Instantiate(hitFX, GetAimLocation(), transform.rotation);
      }

      foreach (var toDestroy in destroyOnHit)
      {
        Destroy(toDestroy);
      }

      Destroy(gameObject, LifeAfterImpact);
    }
  }
}