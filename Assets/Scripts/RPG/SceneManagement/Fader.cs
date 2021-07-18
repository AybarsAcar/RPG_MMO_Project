using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
  public class Fader : MonoBehaviour
  {
    private CanvasGroup _canvasGroup;

    private Coroutine _currentlyRunningCoroutine = null;

    private void Awake()
    {
      _canvasGroup = GetComponent<CanvasGroup>();
      // _canvasGroup.alpha = 0f;
    }

    public void FadeOutImmediate()
    {
      _canvasGroup.alpha = 1f;
    }

    public Coroutine FadeOut(float time)
    {
      return Fade(1f, time);
    }

    public Coroutine FadeIn(float time)
    {
      return Fade(0f, time);
    }

    public Coroutine Fade(float target, float time)
    {
      // Cancel any running coroutines
      if (_currentlyRunningCoroutine != null)
      {
        StopCoroutine(_currentlyRunningCoroutine);
      }

      // Run the FadeOutCoroutine
      _currentlyRunningCoroutine = StartCoroutine(FadeCoroutine(target, time));
      return _currentlyRunningCoroutine;
    }

    private IEnumerator FadeCoroutine(float target, float time)
    {
      while (!Mathf.Approximately(_canvasGroup.alpha, target))
      {
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.unscaledTime / time);
        yield return null; // so we update it every frame
      }
    }
  }
}