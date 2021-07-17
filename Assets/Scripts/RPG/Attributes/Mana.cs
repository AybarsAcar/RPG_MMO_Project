using UnityEngine;

namespace RPG.Attributes
{
  /// <summary>
  /// attached to the player object
  /// tracks the player Mana
  /// </summary>
  public class Mana : MonoBehaviour
  {
    [SerializeField] private float maxMana = 200f;

    [Tooltip("Mana Regen value per second")] [SerializeField]
    private float manaRegenRate = 1;

    public float MaxMana => maxMana;

    private float _currentMana;
    public float CurrentMana => _currentMana;

    private void Awake()
    {
      _currentMana = maxMana;
    }

    private void Update()
    {
      if (_currentMana < maxMana)
      {
        _currentMana += manaRegenRate * Time.deltaTime;

        if (_currentMana > maxMana) _currentMana = maxMana;
      }
    }

    public bool UseMana(float manaToUse)
    {
      if (manaToUse > _currentMana) return false;

      _currentMana -= manaToUse;
      return true;
    }
  }
}