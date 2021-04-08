using System;
using UnityEngine;

namespace Listonos.TimeSystem
{
  public class TimeDisplay : MonoBehaviour
  {
    public TMPro.TextMeshProUGUI Text;
    [Tooltip("How to format the time. Formatted with `DateTime.ToString(Format)`. Can be empty in which case `DateTime.ToString()` is used for formatting.")]
    public string Format = "--MM-dd HH:mm";

    [Tooltip("How to format the result of the Format parameter when the time is paused. The result is formatted with `string.Format(PausedFormat, result)`. Can be empty in which case it is ignored.")]
    public string PausedFormat = "(Paused) {0}";

    private DateTime displayedTime;
    public DateTime DisplayedTime
    {
      get
      {
        return displayedTime;
      }
      private set
      {
        if (value != displayedTime)
        {
          displayedTime = value;
          if (Format.Length != 0)
          {
            var text = DisplayedTime.ToString(Format);
            if (TimeSystem.Instance.TimePassage == TimePassage.Paused && PausedFormat.Length != 0)
            {
              text = string.Format(PausedFormat, text);
            }

            Text.text = text;
          }
          else
          {
            Text.text = DisplayedTime.ToString();
          }
        }
      }
    }

    void Awake()
    {
      Debug.AssertFormat(Text != null, "TimeDisplay behavior expects valid Text reference.");
    }

    // Start is called before the first frame update
    void Start()
    {
      DisplayedTime = TimeSystem.Instance.Time;    
    }

    // Update is called once per frame
    void Update()
    {
      DisplayedTime = TimeSystem.Instance.Time;
    }
  }
}
