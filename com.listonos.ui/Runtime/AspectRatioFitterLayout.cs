using UnityEngine;
using UnityEngine.UI;

namespace Listonos.UI
{
  public enum AspectRatioMode
  {
    None,
    HeightControlsWidth,
    WidthControlsHeight
  }

  public class AspectRatioFitterLayout : MonoBehaviour
  {
    public AspectRatioMode AspectMode;

    public float Ratio = 1.0f;

    private LayoutElement layoutElement;
    private RectTransform rectTransform;

    private bool initialized = false;

    void Awake()
    {
      if (!initialized) Initialize();
    }

    void Start()
    {
      UpdateRect();
    }

    void OnRectTransformDimensionsChange()
    {
      if (!initialized) Initialize();

      if (UpdateRect())
      {
        var parentGO = transform.parent.gameObject;
        var parentRectTransform = parentGO.GetComponent<RectTransform>();
        if (parentRectTransform != null)
        {
          LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
        }
      }
    }

    private bool UpdateRect()
    {
      var rebuildLayout = false;
      if (AspectMode == AspectRatioMode.HeightControlsWidth && rectTransform.sizeDelta.y != 0)
      {
        var height = rectTransform.sizeDelta.y;
        layoutElement.preferredWidth = height * Ratio;
        rebuildLayout = true;
        
      }
      else if (AspectMode == AspectRatioMode.WidthControlsHeight && rectTransform.sizeDelta.x != 0)
      {
        var width = rectTransform.sizeDelta.x;
        layoutElement.preferredHeight = width * Ratio;
        rebuildLayout = true;
      }

      return rebuildLayout;
    }

    private void Initialize()
    {
      initialized = true;

      layoutElement = GetComponent<LayoutElement>();
      Debug.AssertFormat(layoutElement != null, "AspectRatioLayoutElement behavior requires LayoutElement behavior.");
      rectTransform = GetComponent<RectTransform>();
      Debug.AssertFormat(rectTransform != null, "AspectRatioLayoutElement behavior requires RectTransform behavior.");
    }
  }
}
