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

    private const float MaxLifeTime = 15f;
    private const float LifeAfterImpact = 0.1f;

    private void Start()
    {
      transform.LookAt(GetAimLocation());
    }

    private void Update()
    {
      if (_target == null) return;

      if (isHoming && !_target.IsDead)
      {
        transform.LookAt(GetAimLocation());
      }

      transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
      _target = target;
      _damage = damage;
      _instigator = instigator;

      Destroy(gameObject, MaxLifeTime);
    }

    /**
     * returns the body chest of the Character that targets
     * we use capsule colliders height
     * because our collider is the height of our player
     */
    private Vector3 GetAimLocation()
    {
      var targetCapsule = _target.GetComponent<CapsuleCollider>();

      if (targetCapsule == null) return _target.transform.position;

      return _target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

    /**
     * when the projectile collides with the enemy
     */
    private void OnTriggerEnter(Collider other)
    {
      if (_target.IsDead) return;

      if (other.GetComponent<Health>() != _target) return;
      
      onHitSFX.Invoke();

      if (hitFX != null)
      {
        Instantiate(hitFX, GetAimLocation(), transform.rotation);
      }

      _target.TakeDamage(_instigator, _damage);

      foreach (var toDestroy in destroyOnHit)
      {
        Destroy(toDestroy);
      }

      Destroy(gameObject, LifeAfterImpact);
    }
  }
}