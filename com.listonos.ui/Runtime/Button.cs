using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Listonos.UI
{
  public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
  {
    public UnityEngine.UI.Button UnityButton;
    public TMPro.TextMeshProUGUI ButtonText;
    public int PressedOffset = 4;

    private RectTransform ButtonTextTransform;
    private Vector2 NormalButtonTextAnchoredPosition;
    private Vector2 PressedButtonTextAnchoredPosition;

    private bool interactible = true;
    public bool Interactible
    {
      get
      {
        return interactible;
      }
      set
      {
        if (value != interactible)
        {
          interactible = value;
          UnityButton.interactable = interactible;
        }
      }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (Interactible)
      {
        ButtonTextTransform.anchoredPosition = PressedButtonTextAnchoredPosition;
        PressOffsetApplied?.Invoke(this, new PressOffsetEventArgs() { Offset = PressedOffset });
      }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (Interactible)
      {
        ButtonTextTransform.anchoredPosition = NormalButtonTextAnchoredPosition;
        PressOffsetRemoved?.Invoke(this, new PressOffsetEventArgs() { Offset = PressedOffset });
      }
    }

    public class PressOffsetEventArgs : EventArgs
    {
      public int Offset { get; set; }
    }

    public event EventHandler<PressOffsetEventArgs> PressOffsetApplied;
    public event EventHandler<PressOffsetEventArgs> PressOffsetRemoved;


    void Start()
    {
      Debug.AssertFormat(UnityButton != null, "Button behavior is missing UnityButton reference.");
      Interactible = UnityButton.interactable;

      Debug.AssertFormat(ButtonText != null, "Button behavior is missing ButtonText reference.");
      ButtonTextTransform = ButtonText.GetComponent<RectTransform>();
      Debug.AssertFormat(ButtonTextTransform != null, "Button behavior is missing RectTransform on ButtonText reference.");
      NormalButtonTextAnchoredPosition = ButtonTextTransform.anchoredPosition;
      PressedButtonTextAnchoredPosition = new Vector2(NormalButtonTextAnchoredPosition.x, NormalButtonTextAnchoredPosition.y - PressedOffset);
    }
  }
}

