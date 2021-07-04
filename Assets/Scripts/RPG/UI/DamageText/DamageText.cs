using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
  public class DamageText : MonoBehaviour
  {
    [SerializeField] private Text damageText;

    /**
     * called by a Unity Event
     */
    public void DestroyText()
    {
      Destroy(gameObject);
    }

    public void SetValue(float amount)
    {
      damageText.text = $"{amount:0}";
    }
  }
}