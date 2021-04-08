using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Listonos.TimeSystem
{
  public enum TimePassage
  {
    Paused,
    NormalSpeed,
    FastSpeed,
    SuperSpeed,
    CustomSpeed
  }

  [Serializable]
  public struct DateTimeTicksWrapper
  {
    public long Ticks;
  }

  public delegate void OnceTimeCallbackDelegate(DateTime dateTime);
  public delegate bool RepeatedTimeCallbackDelegate(DateTime dateTime, int counter);

  public class OnceCallbackData : IComparable<OnceCallbackData>
  {
    public DateTime time;
    public OnceTimeCallbackDelegate callback;

    public int CompareTo(OnceCallbackData other)
    {
      return DateTime.Compare(time, other.time);
    }
  }

  public class RepeatedCallbackData
  {
    public DateTime startTime;
    public TimeSpan repeatTimeSpan;
    public DateTime nextTriggerTime;
    public int counter;
    public RepeatedTimeCallbackDelegate callback;
  }

  public class TimePassageChangedEventArgs : EventArgs
  {
    public TimePassage TimePassage { get; set; }
  }

  public class TimeSystem : SingletonMonoBehaviour<TimeSystem>
  {
    public DateTimeTicksWrapper StartingTime;
    public TimePassage StartingTimePassage = TimePassage.Paused;

    [Tooltip("Number of simulated seconds that will pass per game time second.")]
    public float NormalSpeedPerSeconds = 60.0f;
    public float FastSpeedMultiplier = 2.0f;
    public float SuperSpeedMultiplier = 4.0f;

    private List<OnceCallbackData> OnceTimeCallbacksAtTime = new List<OnceCallbackData>();
    private List<RepeatedCallbackData> RepeatedCallbacks = new List<RepeatedCallbackData>();

    private DateTime time;
    public DateTime Time
    {
      get
      {
        return time;
      }
      set
      {
        if (value != time)
        {
          time = value;
        }
      }
    }


    private TimePassage timePassage;
    public TimePassage TimePassage
    {
      get
      {
        return timePassage;
      }
      set
      {
        if (value != timePassage)
        {
          timePassage = value;
          TimePassageChanged?.Invoke(this, new TimePassageChangedEventArgs() { TimePassage = TimePassage });
        }
      }
    }


    private float customSpeed = 1.0f;
    public float CustomSpeed
    {
      get
      {
        return customSpeed;
      }
      set
      {
        if (value != customSpeed)
        {
          customSpeed = value;
        }
      }
    }

    public event EventHandler<TimePassageChangedEventArgs> TimePassageChanged;

    public void Start()
    {
      time = new DateTime(StartingTime.Ticks);
      timePassage = StartingTimePassage;
    }

    public void Update()
    {
      Simulate(UnityEngine.Time.deltaTime);
    }

    public OnceCallbackData RequestOneTimeCallbackAtTime(DateTime wantedTime, OnceTimeCallbackDelegate callback)
    {
      var data = new OnceCallbackData() { callback = callback, time = wantedTime };
      if (wantedTime <= Time)
      {
        callback(Time);        
      }
      else
      {
        OnceTimeCallbacksAtTime.Add(data);
        OnceTimeCallbacksAtTime.Sort();
      }
      return data;
    }

    public OnceCallbackData RequestOneTimeCallbackInTime(TimeSpan timeSpanFromNow, OnceTimeCallbackDelegate callback)
    {
      return RequestOneTimeCallbackAtTime(Time + timeSpanFromNow, callback);
    }

    public RepeatedCallbackData RequestRepeatedCallbackStartNow(TimeSpan repeatTimeSpan, RepeatedTimeCallbackDelegate callback)
    {
      return RequestRepeatedCallback(Time, repeatTimeSpan, callback);
    }

    public RepeatedCallbackData RequestRepeatedCallback(DateTime startTime, TimeSpan repeatTimeSpan, RepeatedTimeCallbackDelegate callback)
    {
      var data = new RepeatedCallbackData() { callback = callback, startTime = startTime, repeatTimeSpan = repeatTimeSpan, counter = 0 };
      data.nextTriggerTime = startTime + repeatTimeSpan;

      if (!RepeatedCallbackSimulation(data, Time))
      {
        RepeatedCallbacks.Add(data);
      }

      return data;
    }

    public bool StopOneTimeCallback(OnceCallbackData data)
    {
      return OnceTimeCallbacksAtTime.Remove(data);
    }

    public bool StopRepeatedCallback(RepeatedCallbackData data)
    {
      return RepeatedCallbacks.Remove(data);
    }

    public void Simulate(float deltaTime)
    {
      SimulateTime(deltaTime);
      SimulateOnceCallbacks();
      SimulateRepeatedCallbacks();
    }

    private void SimulateTime(float deltaTime)
    {
      if (TimePassage == TimePassage.Paused)
      {
        return;
      }

      var speed = NormalSpeedPerSeconds;
      if (TimePassage == TimePassage.FastSpeed)
      {
        speed *= FastSpeedMultiplier;
      }
      else if (TimePassage == TimePassage.SuperSpeed)
      {
        speed *= SuperSpeedMultiplier;
      }
      else if (TimePassage == TimePassage.CustomSpeed)
      {
        speed = CustomSpeed;
      }

      Time = Time.AddSeconds(speed * deltaTime);
    }

    private void SimulateOnceCallbacks()
    {
      // Because the callbacks are sorted the first `if` can exit early and therefore do removal via new list
      List<int> toRemove = new List<int>();
      for (int i = 0; i < OnceTimeCallbacksAtTime.Count; i++)
      {
        var oneTimeCallbackAtTime = OnceTimeCallbacksAtTime[i];
        if (Time >= oneTimeCallbackAtTime.time)
        {
          oneTimeCallbackAtTime.callback(Time);
          toRemove.Add(i);
        }
        else
        {
          break;
        }
      }

      // Reverse iteration to make RemoteAt safe
      for (int i = toRemove.Count - 1; i >= 0; i--)
      {
        var index = toRemove[i];
        OnceTimeCallbacksAtTime.RemoveAt(index);
      }
    }

    private void SimulateRepeatedCallbacks()
    {
      List<int> toRemove = new List<int>();
      for (int i = 0; i < RepeatedCallbacks.Count; i++)
      {
        var repeatedCallbackData = RepeatedCallbacks[i];
        if (RepeatedCallbackSimulation(repeatedCallbackData, Time))
        {
          toRemove.Add(i);
        }
      }

      // Reverse iteration to make `RemoteAt` safe
      for (int i = toRemove.Count - 1; i >= 0; i--)
      {
        var index = toRemove[i];
        RepeatedCallbacks.RemoveAt(index);
      }
    }

    private bool RepeatedCallbackSimulation(RepeatedCallbackData data, DateTime currentTime)
    {
      var finished = false;

      while (data.nextTriggerTime <= currentTime)
      {
        if (data.callback(currentTime, data.counter))
        {
          finished = true;
          break;
        }

        data.counter++;
        data.nextTriggerTime += data.repeatTimeSpan;
      }

      return finished;
    }
  }
}
