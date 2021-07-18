using RPG.SceneManagement;
using RPG.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
  public class MainMenuUI : MonoBehaviour
  {
    [SerializeField] private TMP_InputField newGameNameField;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;

    private LazyValue<SavingWrapper> _savingWrapper;

    private void Awake()
    {
      _savingWrapper = new LazyValue<SavingWrapper>(() => FindObjectOfType<SavingWrapper>());
    }

    private void Start()
    {
      continueButton.onClick.AddListener(() => _savingWrapper.Value.ContinueGame());
      newGameButton.onClick.AddListener(() => _savingWrapper.Value.NewGame(newGameNameField.text));
      quitButton.onClick.AddListener(HandleQuit);
    }

    /// <summary>
    /// so allows us to debug without building
    /// and to see the functionality in the editor mode
    /// </summary>
    private void HandleQuit()
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}