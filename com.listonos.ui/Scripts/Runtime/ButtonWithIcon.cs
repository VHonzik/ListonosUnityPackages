using UnityEngine;

namespace Listonos.UI
{
  public class ButtonWithIcon : MonoBehaviour
  {
    public RectTransform IconTransform;

    private Button Button;

    private Vector2 NormalIconAnchoredPosition;
    private Vector2 PressedIconAnchoredPosition;

    // Start is called before the first frame update
    void Start()
    {
      Button = GetComponent<Button>();
      Debug.AssertFormat(Button != null, "ButtonWithIcon behavior should be coupled with Button behavior on the same game object.");
      Button.PressOffsetApplied += Button_PressOffsetApplied;
      Button.PressOffsetRemoved += Button_PressOffsetRemoved;

      Debug.AssertFormat(IconTransform != null, "ButtonWithIcon behavior is missing RectTransform of the Icon.");
      NormalIconAnchoredPosition = IconTransform.anchoredPosition;
    }

    private void Button_PressOffsetApplied(object sender, Button.PressOffsetEventArgs e)
    {
      PressedIconAnchoredPosition = new Vector2(NormalIconAnchoredPosition.x, NormalIconAnchoredPosition.y - e.Offset);
      IconTransform.anchoredPosition = PressedIconAnchoredPosition;
    }

    private void Button_PressOffsetRemoved(object sender, Button.PressOffsetEventArgs e)
    {
      IconTransform.anchoredPosition = NormalIconAnchoredPosition;
    }
  }
}
