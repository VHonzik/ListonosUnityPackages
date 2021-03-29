using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Listonos.Buttons
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
          if (UnityButton)
          {
            UnityButton.interactable = interactible;
          }
        }
      }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (ButtonTextTransform && Interactible)
      {
        ButtonTextTransform.anchoredPosition = PressedButtonTextAnchoredPosition;
      }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (ButtonTextTransform && Interactible)
      {
        ButtonTextTransform.anchoredPosition = NormalButtonTextAnchoredPosition;
      }
    }

    // Start is called before the first frame update
    void Start()
    {
      if (UnityButton)
      {
        Interactible = UnityButton.interactable;
      }

      if (ButtonText)
      {
        ButtonTextTransform = ButtonText.GetComponent<RectTransform>();
        if (ButtonTextTransform)
        {
          NormalButtonTextAnchoredPosition = ButtonTextTransform.anchoredPosition;
          PressedButtonTextAnchoredPosition = new Vector2(ButtonTextTransform.anchoredPosition.x, ButtonTextTransform.anchoredPosition.y - PressedOffset);
        }
      }
    }
  }
}

