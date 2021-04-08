using UnityEngine;

namespace Listonos.TimeSystem
{
  public class TimeControls : MonoBehaviour
  {
    private TimePassage previousNotPausedPassage = TimePassage.NormalSpeed;

    // Start is called before the first frame update
    void Start()
    {
      TimeSystem.Instance.TimePassageChanged += Instance_TimePassageChanged;
    }

    private void Instance_TimePassageChanged(object sender, TimePassageChangedEventArgs e)
    {
      if (e.TimePassage != TimePassage.Paused)
      {
        previousNotPausedPassage = e.TimePassage;
      }
    }

    public void PlayPause()
    {
      if (TimeSystem.Instance.TimePassage == TimePassage.Paused)
      {
        TimeSystem.Instance.TimePassage = previousNotPausedPassage;
      }
      else
      {
        TimeSystem.Instance.TimePassage = TimePassage.Paused;
      }
    }

    public void FastSpeed()
    {
      TimeSystem.Instance.TimePassage = TimePassage.FastSpeed;
    }

    public void SuperSpeed()
    {
      TimeSystem.Instance.TimePassage = TimePassage.SuperSpeed;
    }
  }
}
