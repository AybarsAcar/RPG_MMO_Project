using UnityEngine;

namespace RPG.UI
{
  public class ShowHideUI : MonoBehaviour
  {
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
    [SerializeField] private GameObject uiContainer = null;

    private bool _isUIActive;

    // Start is called before the first frame update
    private void Start()
    {
      uiContainer.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
      if (Input.GetKeyDown(toggleKey))
      {
        Close();
      }
    }

    /// <summary>
    /// called by the quit button on click on Unity Editor
    /// </summary>
    public void Close()
    {
      uiContainer.SetActive(!uiContainer.activeSelf);
    }
  }
}