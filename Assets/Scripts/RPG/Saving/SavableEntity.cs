using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
  [ExecuteAlways]
  public class SavableEntity : MonoBehaviour
  {
    // so the static value will not change
    private static Dictionary<string, SavableEntity>
      _globalSavableEntityLookup = new Dictionary<string, SavableEntity>();

    [SerializeField] private string uniqueIdentifier = "";

#if UNITY_EDITOR
    private void Update()
    {
      if (Application.isPlaying) return;
      if (string.IsNullOrEmpty(gameObject.scene.path)) return; // so we don't assign in Prefab mode

      var serializedObject = new SerializedObject(this);

      var serializedProperty = serializedObject.FindProperty("uniqueIdentifier");

      if (string.IsNullOrEmpty(serializedProperty.stringValue) || !IsUnique(serializedProperty.stringValue))
      {
        serializedProperty.stringValue = Guid.NewGuid().ToString();
        serializedObject.ApplyModifiedProperties(); // make sure to apply the changes made        
      }

      // update the lookup map each time we make a change in the editor
      // so each SavableEntity component will register itself to this dictionary
      _globalSavableEntityLookup[serializedProperty.stringValue] = this;
    }

#endif

    public string GetUniqueIdentifier()
    {
      return uniqueIdentifier;
    }

    /**
     * we are returning object type which is the most general type - root object
     * we can return anything from this one
     */
    public object CaptureState()
    {
      var state = new Dictionary<string, object>();

      foreach (var savable in GetComponents<ISavable>())
      {
        state[savable.GetType().ToString()] = savable.CaptureState();
      }

      return state;
      // return new SerializableVector3(transform.position);
    }

    public void RestoreState(object state)
    {
      var stateDict = (Dictionary<string, object>) state;

      foreach (var savable in GetComponents<ISavable>())
      {
        var key = savable.GetType().ToString();

        if (stateDict.ContainsKey(key))
        {
          // restore state for each component
          savable.RestoreState(stateDict[key]);
        }
      }
    }

    /**
     * checks if the GUID is unique for the component
     * to prevent duplicate ids when a gameObject is copied
     */
    private bool IsUnique(string id)
    {
      if (!_globalSavableEntityLookup.ContainsKey(id)) return true;

      if (_globalSavableEntityLookup[id] == this) return true;

      if (_globalSavableEntityLookup[id] == null)
      {
        _globalSavableEntityLookup.Remove(id);
        return true;
      }

      // edge case when designer manually updates the guid in the editor
      // this is a cleanup method
      if (_globalSavableEntityLookup[id].GetUniqueIdentifier() != id)
      {
        _globalSavableEntityLookup.Remove(id);
        return true;
      }

      return false;
    }
  }
}