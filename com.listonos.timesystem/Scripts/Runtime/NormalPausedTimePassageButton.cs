using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Listonos.TimeSystem
{
  public class NormalPausedTimePassageButton : MonoBehaviour
  {
    public Image Icon;
    public Sprite NormalPassageSprite;
    public Sprite PausedPassageSprite;

    // Start is called before the first frame update
    void Start()
    {
      Icon.sprite = TimeSystem.Instance.TimePassage == TimePassage.Paused ? NormalPassageSprite : PausedPassageSprite;
      TimeSystem.Instance.TimePassageChanged += TimePassageChanged;
    }

    private void TimePassageChanged(object sender, TimePassageChangedEventArgs e)
    {
      Icon.sprite = e.TimePassage == TimePassage.Paused ? NormalPassageSprite : PausedPassageSprite;
    }

  }
}
