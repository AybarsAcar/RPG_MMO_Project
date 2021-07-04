using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
  /// <summary>
  /// player interacts with this class to save state within the game
  /// </summary>
  public class SavingWrapper : MonoBehaviour
  {
    private const string DefaultSaveFile = "save";
    private const float FadeInTime = 2f;

    private void Awake()
    {
      StartCoroutine(LoadLastScene());
    }

    private IEnumerator LoadLastScene()
    {
      yield return GetComponent<SavingSystem>().LoadLastScene(DefaultSaveFile);

      var fader = FindObjectOfType<Fader>();

      // Fade out completely
      fader.FadeOutImmediate();

      //Fade in
      yield return fader.FadeIn(FadeInTime);
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
      GetComponent<SavingSystem>().Save(DefaultSaveFile);
    }

    public void LoadGameState()
    {
      GetComponent<SavingSystem>().Load(DefaultSaveFile);
    }

    /**
     * deletes the saving file
     * for debugging
     */
    private void DeleteSaveFile()
    {
      GetComponent<SavingSystem>().Delete(DefaultSaveFile);
    }
  }
}