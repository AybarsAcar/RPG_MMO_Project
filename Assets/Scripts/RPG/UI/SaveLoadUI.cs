using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
  /// <summary>
  /// Script is attached to the Load Menu Game Object
  /// </summary>
  public class SaveLoadUI : MonoBehaviour
  {
    [Tooltip("The game object that contains the load buttons")] [SerializeField]
    private Transform contentRoot;

    // the buttons are fetched from PlayerPrefs for each unique gameplay
    [Tooltip("Button Prefab that will load any unique gameplay save data")] [SerializeField]
    private GameObject loadButtonPrefab;

    /// <summary>
    /// runs when the game object is activated
    /// so when we go into this component
    /// </summary>
    private void OnEnable()
    {
      var savingWrapper = FindObjectOfType<SavingWrapper>();
      if (savingWrapper == null) return;

      // clear the children
      foreach (Transform child in contentRoot.transform)
      {
        Destroy(child.gameObject);
      }

      // instantiate as the child
      foreach (var saveFile in savingWrapper.ListSaves())
      {
        var buttonInstance = Instantiate(loadButtonPrefab, contentRoot);

        buttonInstance.GetComponentInChildren<TMP_Text>().SetText(saveFile);

        buttonInstance.GetComponent<Button>().onClick.AddListener(() => savingWrapper.LoadGame(saveFile));
      }
    }
  }
}