using RPG.Control;
using RPG.Core.Util;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
  /// <summary>
  /// attached to the pause window which is a child of the Pause menu
  /// responsible to pause the game when the window is enabled
  /// </summary>
  public class PauseMenuUI : MonoBehaviour
  {
    [SerializeField] private Button saveButton;
    [SerializeField] private Button saveAndQuitButton;
    
    private PlayerController _playerController;

    private void Start()
    {
      _playerController = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<PlayerController>();
      
      saveButton.onClick.AddListener(Save);
      saveAndQuitButton.onClick.AddListener(SaveAndQuit);
    }

    private void OnEnable()
    {
      _playerController.enabled = false;
      Time.timeScale = 0;
    }

    private void OnDisable()
    {
      _playerController.enabled = true;
      Time.timeScale = 1;
    }

    private void Save()
    {
      var savingWrapper = FindObjectOfType<SavingWrapper>();
      savingWrapper.SaveGameState();
    }

    private void SaveAndQuit()
    {
      var savingWrapper = FindObjectOfType<SavingWrapper>();
      savingWrapper.SaveGameState();

      savingWrapper.LoadMainMenu();
    }
  }
}