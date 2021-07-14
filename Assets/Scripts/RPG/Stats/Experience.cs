using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
  public class Experience : MonoBehaviour, ISavable
  {
    private float _experiencePoints;
    public float ExperiencePoints => _experiencePoints;

    public event Action OnExperienceGained;

    private void Update()
    {
      if (Input.GetKey(KeyCode.E))
      {
        GainExperience(Time.deltaTime * 100);
      }
    }

    public void GainExperience(float experience)
    {
      _experiencePoints += experience;
      OnExperienceGained();
    }

    /**
     * passes experience points to Saving System
     */
    public object CaptureState()
    {
      return _experiencePoints;
    }

    /**
     * grabs and assigns the passed in state from saving system to experience points
     */
    public void RestoreState(object state)
    {
      _experiencePoints = (float) state;
    }
  }
}