using UnityEngine;

namespace Listonos.UI
{
  public class ButtonWithShortcut : MonoBehaviour
  {
    public KeyCode Shortcut = KeyCode.None;
    public string ShortcutDisplayedText;

    public TMPro.TextMeshProUGUI ShortcutText;

    private RectTransform ShortcutTextTransform;
    private Vector2 NormalShortcutAnchoredPosition;
    private Vector2 PressedShortcutAnchoredPosition;

    private Button Button;
    private UnityEngine.UI.Button UnityButton;

    void Start()
    {
      Debug.AssertFormat(ShortcutText != null, "ButtonWithShortcut behavior is missing ShortcutText reference.");
      if (ShortcutDisplayedText.Length > 0)
      {
        ShortcutText.text = ShortcutDisplayedText;
      }
      else
      {
        ShortcutText.text = Shortcut.ToString();
      }

      ShortcutTextTransform = ShortcutText.GetComponent<RectTransform>();
      Debug.AssertFormat(ShortcutTextTransform != null, "ButtonWithShortcut behavior is missing RectTransform.");
      NormalShortcutAnchoredPosition = ShortcutTextTransform.anchoredPosition;
      if (Shortcut == KeyCode.None)
      {
        ShortcutText.gameObject.SetActive(false);
      }

      Button = GetComponent<Button>();
      Debug.AssertFormat(Button != null, "ButtonWithShortcut behavior should be coupled with Button behavior.");
      Button.PressOffsetApplied += Button_PressOffsetApplied;
      Button.PressOffsetRemoved += Button_PressOffsetRemoved;

      UnityButton = GetComponent<UnityEngine.UI.Button>();
      Debug.AssertFormat(Button != null, "ButtonWithShortcut behavior should be coupled with Unity's Button behavior.");
    }

    private void Button_PressOffsetApplied(object sender, Button.PressOffsetEventArgs e)
    {
      PressedShortcutAnchoredPosition = new Vector2(NormalShortcutAnchoredPosition.x, NormalShortcutAnchoredPosition.y - e.Offset);
      ShortcutTextTransform.anchoredPosition = PressedShortcutAnchoredPosition;
    }

    private void Button_PressOffsetRemoved(object sender, Button.PressOffsetEventArgs e)
    {
      ShortcutTextTransform.anchoredPosition = NormalShortcutAnchoredPosition;
    }

    void Update()
    {
      if (gameObject.activeInHierarchy && Shortcut != KeyCode.None && Input.GetKeyDown(Shortcut) && Button.Interactible)
      {
        UnityButton.onClick.Invoke();
      }
    }
  }


}
