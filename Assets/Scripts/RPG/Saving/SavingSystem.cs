using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
  /// <summary>
  /// This component provides the interface to the saving system. It provides
  /// methods to save and restore a scene
  ///
  /// This component should be created once and shared between all subsequent scenes.
  /// </summary>
  public class SavingSystem : MonoBehaviour
  {
    /// <summary>
    /// loads the last scene the player was on
    /// called from the Wrapper on Awake so the game loads from the last scene the player was in
    /// </summary>
    /// <param name="saveFile"></param>
    /// <returns></returns>
    public IEnumerator LoadLastScene(string saveFile)
    {
      // get state
      var state = LoadFile(saveFile);

      var buildIndex = SceneManager.GetActiveScene().buildIndex;
      if (state.ContainsKey("lastSceneBuildIndex"))
      {
        // restore the last scene
        buildIndex = (int) state["lastSceneBuildIndex"];
      }

      // restore the last scene if we are not already on that scene
      yield return SceneManager.LoadSceneAsync(buildIndex);

      // restore the state of that scene
      RestoreState(state);
    }

    public void Save(string saveFile)
    {
      var state = LoadFile(saveFile); // so it doesnt override

      // capture the state into the save file
      CaptureState(state);

      SaveFile(saveFile, state);
    }

    public void Load(string saveFile)
    {
      RestoreState(LoadFile(saveFile));
    }

    /// <summary>
    /// returns the list of save files in the directory
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> ListSaveFiles()
    {
      foreach (var path in Directory.EnumerateFiles(Application.persistentDataPath))
      {
        if (Path.GetExtension(path) == ".sav")
        {
          yield return Path.GetFileName(path);
        }
      }
    }

    private Dictionary<string, object> LoadFile(string file)
    {
      var path = GetPathFromSaveFile(file);

      if (!File.Exists(path)) return new Dictionary<string, object>(); // if no file return an empty state

      using var fileStream = File.Open(path, FileMode.Open);

      var formatter = new BinaryFormatter();

      return formatter.Deserialize(fileStream) as Dictionary<string, object>;
    }

    private void SaveFile(string file, Dictionary<string, object> state)
    {
      using var fileStream = File.Open(GetPathFromSaveFile(file), FileMode.Create);

      var formatter = new BinaryFormatter();

      formatter.Serialize(fileStream, state);
    }

    private void RestoreState(IReadOnlyDictionary<string, object> state)
    {
      foreach (var savable in FindObjectsOfType<SavableEntity>())
      {
        var id = savable.GetUniqueIdentifier();
        if (state.ContainsKey(id))
        {
          savable.RestoreState(state[id]);
        }
      }
    }


    /// <summary>
    /// update the dictionary passed in
    /// </summary>
    /// <param name="state"></param>
    private void CaptureState(IDictionary<string, object> state)
    {
      foreach (var savable in FindObjectsOfType<SavableEntity>())
      {
        state[savable.GetUniqueIdentifier()] = savable.CaptureState();
      }

      // serialise the scene as well
      state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
    }

    private string GetPathFromSaveFile(string saveFile)
    {
      return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
    }

    public void Delete(string saveFile)
    {
      File.Delete(GetPathFromSaveFile(saveFile));
    }
  }
}