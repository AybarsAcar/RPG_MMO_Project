using RPG.Core.Util;
using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
  public class DialogueUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI aiText;
    [SerializeField] private TextMeshProUGUI conversantName;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Transform choiceRoot; // Player response choices
    [SerializeField] private GameObject aiResponse; // AI Response part

    [SerializeField] private GameObject choiceButtonPrefab;

    private PlayerConversant _playerConversant;

    private void Start()
    {
      _playerConversant = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<PlayerConversant>();

      _playerConversant.OnConversationUpdated += UpdateUI;

      nextButton.onClick.AddListener(() => _playerConversant.Next());
      quitButton.onClick.AddListener(() => _playerConversant.Quit());

      UpdateUI();
    }

    private void UpdateUI()
    {
      gameObject.SetActive(_playerConversant.IsActive());

      if (!_playerConversant.IsActive()) return;

      conversantName.text = _playerConversant.GetCurrentConversantName();
      
      aiResponse.SetActive(!_playerConversant.IsPlayerChoosing);
      choiceRoot.gameObject.SetActive(_playerConversant.IsPlayerChoosing);

      if (_playerConversant.IsPlayerChoosing)
      {
        BuildChoiceList();
      }
      else
      {
        aiText.text = _playerConversant.GetText();
        nextButton.gameObject.SetActive(_playerConversant.HasNext());
      }
    }

    private void BuildChoiceList()
    {
      // destroy the children of the root initially
      foreach (Transform child in choiceRoot)
      {
        Destroy(child.gameObject);
      }

      // recreate the buttons
      foreach (var choice in _playerConversant.GetChoices())
      {
        var choiceInstance = Instantiate(choiceButtonPrefab, choiceRoot);

        var tmp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = choice.Text;

        var button = choiceInstance.GetComponentInChildren<Button>();
        button.onClick.AddListener(() => { _playerConversant.SelectChoice(choice); });
      }
    }
  }
}