using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
  /// <summary>
  /// player interacts with this class to save state within the game
  /// </summary>
  public class SavingWrapper : MonoBehaviour
  {
    private const string CurrentSaveKey = "currentSaveName";

    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeOutTime = 0.2f;

    public void ContinueGame()
    {
      StartCoroutine(LoadLastScene());
    }

    public void NewGame(string saveFile)
    {
      SetCurrentSave(saveFile);
      StartCoroutine(LoadFirstScene());
    }

    public void LoadGame(string saveFile)
    {
      SetCurrentSave(saveFile);
      ContinueGame();
    }
    

    private void SetCurrentSave(string saveFile)
    {
      PlayerPrefs.SetString(CurrentSaveKey, saveFile);
    }

    private string GetCurrentSave()
    {
      return PlayerPrefs.GetString(CurrentSaveKey);
    }

    private IEnumerator LoadFirstScene()
    {
      var fader = FindObjectOfType<Fader>();

      // Fade out completely
      yield return fader.FadeOut(fadeOutTime);

      yield return SceneManager.LoadSceneAsync(1);

      //Fade in
      yield return fader.FadeIn(fadeInTime);
    }

    private IEnumerator LoadLastScene()
    {
      var fader = FindObjectOfType<Fader>();

      // Fade out completely
      yield return fader.FadeOut(fadeOutTime);

      yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());

      //Fade in
      yield return fader.FadeIn(fadeInTime);
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.P))
      {
        SaveGameState();
      }

      if (Input.GetKeyDown(KeyCode.L))
      {
        LoadGameState();
      }

      if (Input.GetKeyDown(KeyCode.Alpha0))
      {
        DeleteSaveFile();
      }
    }

    public void SaveGameState()
    {
      GetComponent<SavingSystem>().Save(GetCurrentSave());
    }

    public void LoadGameState()
    {
      GetComponent<SavingSystem>().Load(GetCurrentSave());
    }

    /// <summary>
    /// deletes the saving file
    /// for debugging
    /// </summary>
    private void DeleteSaveFile()
    {
      GetComponent<SavingSystem>().Delete(GetCurrentSave());
    }

    public IEnumerable<string> ListSaves()
    {
      return GetComponent<SavingSystem>().ListSaveFiles();
    }
  }
}