using RPG.Core.Util;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
  public class PlayerBalanceUI : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI balanceField;

    private PlayerBalance _playerBalance;
    
    private void Start()
    {
      _playerBalance = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<PlayerBalance>();

      if (_playerBalance != null)
      {
        _playerBalance.ONChange += RefreshUI;
      }
      
      RefreshUI();
    }

    
    /// <summary>
    /// called everytime the balance is updated
    /// this method is subscribes to the player balance change
    /// </summary>
    private void RefreshUI()
    {
      balanceField.text = $"${_playerBalance.CurrentBalance:n}";
    }
  }
}