using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
  public class Weapon : MonoBehaviour
  {
    [SerializeField] private UnityEvent onHitSFX; // sound effect for the weapon hit
    
    /**
     * to call to weapon from fighter
     * and trigger unity events
     */
    public void OnHit()
    {
      onHitSFX.Invoke();
    }
  }
}