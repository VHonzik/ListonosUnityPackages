using System;
using UnityEngine;

namespace Listonos.TimeSystem
{
  public class TimeDisplay : MonoBehaviour
  {
    public TMPro.TextMeshProUGUI Text;
    [Tooltip("Format to display the time with. Uses .NET date and time format strings internally.")]
    public string Format = "--MM-dd HH:mm:ss";

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
          if (Format.Length == 0)
          {
            Text.text = DisplayedTime.ToString();
          }
          else
          {
            Text.text = DisplayedTime.ToString(Format);
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
