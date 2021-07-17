using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Attributes
{
  /// <summary>
  /// attached to the player object
  /// tracks the player Mana
  /// </summary>
  public class Mana : MonoBehaviour, ISavable
  {
    private LazyValue<float> _currentMana;

    public float MaxMana => GetComponent<BaseStats>().GetStat(Stat.Mana);
    private float ManaRegenRate => GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate);

    public float CurrentMana => _currentMana.Value;

    private void Awake()
    {
      _currentMana = new LazyValue<float>(() => MaxMana);
    }

    private void Update()
    {
      if (_currentMana.Value < MaxMana)
      {
        _currentMana.Value += ManaRegenRate * Time.deltaTime;

        if (_currentMana.Value > MaxMana) _currentMana.Value = MaxMana;
      }
    }

    public bool UseMana(float manaToUse)
    {
      if (manaToUse > _currentMana.Value) return false;

      _currentMana.Value -= manaToUse;
      return true;
    }

    public object CaptureState()
    {
      return _currentMana.Value;
    }

    public void RestoreState(object state)
    {
      _currentMana.Value = (float) state;
    }
  }
}