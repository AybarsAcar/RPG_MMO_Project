using UnityEngine;

namespace RPG.UI
{
  public class ShowHideUI : MonoBehaviour
  {
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
    [SerializeField] private GameObject uiContainer = null;

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
        uiContainer.SetActive(!uiContainer.activeSelf);
      }
    }

    /// <summary>
    /// called by the quit button on click on Unity Editor
    /// </summary>
    public void Close()
    {
      uiContainer.SetActive(false);
    }
  }
}