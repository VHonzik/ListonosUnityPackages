using System;
using System.Collections;
using System.Collections.Generic;
using Listonos.TimeSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeSystemTests
{
  private TimeSystem CreateTimeSystem(TimePassage startingPimePassage, DateTime startingTime)
  {
    var timeSystemGO = new GameObject("TimeSystem");

    var timeSystem = timeSystemGO.AddComponent<TimeSystem>();
    timeSystem.StartingTimePassage = startingPimePassage;
    timeSystem.StartingTime = new DateTimeTicksWrapper() { Ticks = startingTime.Ticks };

    return timeSystem;
  }

  private TimeSystem CreateTimeSystemPaused(DateTime startingTime)
  {
    return CreateTimeSystem(TimePassage.Paused, startingTime);
  }

  private TimeSystem CreateTimeSystemPausedNow()
  {
    return CreateTimeSystem(TimePassage.Paused, DateTime.Now);
  }

  private void AssertDateTimeEqualWithMilisecondsTolerance(DateTime dateTimeA, DateTime dateTimeB, double tolerance = 500.0)
  {
    var difference = (dateTimeB - dateTimeA).Duration().TotalMilliseconds;
    Assert.LessOrEqual(difference, tolerance);
  }

  private void SimulateTimeSystemForSeconds(TimeSystem timeSystem, float seconds, float fps = 60.0f)
  {
    var timer = 0.0f;
    var spf = 1.0f / fps;

    while (timer <= seconds)
    {
      if (timer + spf > seconds)
      {
        timeSystem.Simulate(seconds - timer);
      }
      else
      {
        timeSystem.Simulate(spf);
      }
      timer += spf;
    }
  }

  private void SimulateTimeSystemForFrames(TimeSystem timeSystem, int frameCount, float fps = 60.0f)
  {
    var spf = 1.0f / fps;
    for (int i = 0; i < frameCount; i++)
    {
      timeSystem.Simulate(spf);
    }
  }


  [Test]
  public void StartingTime()
  {
    var fixedTime = DateTime.Now;
    var timeSystem = CreateTimeSystemPaused(fixedTime);
    timeSystem.Start();

    Assert.AreEqual(timeSystem.Time, fixedTime);
  }

  [Test]
  public void Singleton()
  {
    var timeSystem = CreateTimeSystemPausedNow();
    Assert.AreEqual(timeSystem, TimeSystem.Instance);
  }

  [Test]
  public void PausingTimePassage()
  {
    var startingTime = DateTime.Now;
    var timeSystem = CreateTimeSystem(TimePassage.NormalSpeed, startingTime);
    timeSystem.Start();

    SimulateTimeSystemForFrames(timeSystem, 5);

    Assert.Greater(timeSystem.Time, startingTime);
    timeSystem.TimePassage = TimePassage.Paused;
    var timeAfterPausing = timeSystem.Time;

    SimulateTimeSystemForFrames(timeSystem, 5);

    Assert.AreEqual(timeSystem.Time, timeAfterPausing);
  }

  [Test]
  public void TimePassageSpeeds()
  {
    var startingTime = DateTime.Now;
    var timeSystem = CreateTimeSystem(TimePassage.NormalSpeed, startingTime);
    timeSystem.NormalSpeedPerSeconds = 10.0f;
    timeSystem.Start();

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    timeSystem.TimePassage = TimePassage.Paused;
    AssertDateTimeEqualWithMilisecondsTolerance(timeSystem.Time, startingTime.AddSeconds(10.0));

    startingTime = timeSystem.Time;
    timeSystem.FastSpeedMultiplier = 2.0f;
    timeSystem.TimePassage = TimePassage.FastSpeed;

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    timeSystem.TimePassage = TimePassage.Paused;
    AssertDateTimeEqualWithMilisecondsTolerance(timeSystem.Time, startingTime.AddSeconds(20.0));

    startingTime = timeSystem.Time;
    timeSystem.SuperSpeedMultiplier = 4.0f;
    timeSystem.TimePassage = TimePassage.SuperSpeed;

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    timeSystem.TimePassage = TimePassage.Paused;
    AssertDateTimeEqualWithMilisecondsTolerance(timeSystem.Time, startingTime.AddSeconds(40.0));
  }

  [Test]
  public void RequestOneTimeCallbackAtTime()
  {
    var startingTime = DateTime.Now;
    var timeSystem = CreateTimeSystemPaused(startingTime);
    timeSystem.NormalSpeedPerSeconds = 10.0f;
    timeSystem.Start();

    var timeSpan = new TimeSpan(0, 0, 5);
    var expectCallbackTime = startingTime + timeSpan;

    var atTimeCallbackCounter = 0;
    DateTime atTimeCallbackTime = new DateTime();
    timeSystem.RequestOneTimeCallbackAtTime(expectCallbackTime, (DateTime) =>
    {
      atTimeCallbackCounter++;
      atTimeCallbackTime = DateTime;
    });

    var inTimeCallbackCounter = 0;
    DateTime inTimeCallbackTime = new DateTime();
    timeSystem.RequestOneTimeCallbackInTime(timeSpan, (DateTime) =>
    {
      inTimeCallbackCounter++;
      inTimeCallbackTime = DateTime;
    });

    timeSystem.TimePassage = TimePassage.NormalSpeed;
    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    Assert.AreEqual(atTimeCallbackCounter, 1);
    AssertDateTimeEqualWithMilisecondsTolerance(atTimeCallbackTime, expectCallbackTime);

    Assert.AreEqual(inTimeCallbackCounter, 1);
    AssertDateTimeEqualWithMilisecondsTolerance(inTimeCallbackTime, expectCallbackTime);
  }

  [Test]
  public void RepeatedCallback()
  {
    var startingTime = DateTime.Now;
    var timeSystem = CreateTimeSystemPaused(startingTime);
    timeSystem.NormalSpeedPerSeconds = 10.0f;
    timeSystem.Start();

    var timeSpan = new TimeSpan(0, 0, 8);
    var receivedCounter = -1;
    var receivedDateTime = new DateTime();
    timeSystem.RequestRepeatedCallback(timeSystem.Time, timeSpan, (dateTime, counter) =>
    {
      receivedDateTime = dateTime;
      receivedCounter = counter;
      if (counter == 1)
      {
        return true;
      }
      else
      {
        return false;
      }

    });

    timeSystem.TimePassage = TimePassage.NormalSpeed;
    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    Assert.AreEqual(receivedCounter, 0);
    AssertDateTimeEqualWithMilisecondsTolerance(receivedDateTime, startingTime + timeSpan);

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    Assert.AreEqual(receivedCounter, 1);

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    Assert.AreEqual(receivedCounter, 1);
  }

  [Test]
  public void RemoveOneTimeCallback()
  {
    var startingTime = DateTime.Now;
    var timeSystem = CreateTimeSystemPaused(startingTime);
    timeSystem.NormalSpeedPerSeconds = 10.0f;
    timeSystem.Start();

    var timeSpan = new TimeSpan(0, 0, 20);
    var counter = 0;
    var receivedDateTime = new DateTime();
    var data = timeSystem.RequestOneTimeCallbackInTime(timeSpan, (dateTime) =>
    {
      receivedDateTime = dateTime;
      counter++;
    });

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    Assert.AreEqual(counter, 0);

    Assert.IsTrue(timeSystem.StopOneTimeCallback(data));

    SimulateTimeSystemForSeconds(timeSystem, 2.0f);

    Assert.AreEqual(counter, 0);
  }

  [Test]
  public void RemoveRepeatedCallback()
  {
    var startingTime = DateTime.Now;
    var timeSystem = CreateTimeSystemPaused(startingTime);
    timeSystem.NormalSpeedPerSeconds = 10.0f;
    timeSystem.Start();

    var timeSpan = new TimeSpan(0, 0, 20);
    var receivedCounter = -1;
    var receivedDateTime = new DateTime();
    var data = timeSystem.RequestRepeatedCallback(timeSystem.Time, timeSpan, (dateTime, counter) =>
    {
      receivedDateTime = dateTime;
      receivedCounter = counter;
      return false;
    });

    SimulateTimeSystemForSeconds(timeSystem, 1.0f);

    Assert.AreEqual(receivedCounter, -1);

    Assert.IsTrue(timeSystem.StopRepeatedCallback(data));

    SimulateTimeSystemForSeconds(timeSystem, 2.0f);

    Assert.AreEqual(receivedCounter, -1);
  }
}
