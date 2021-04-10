using System;
using NUnit.Framework;
using UnityEngine;

namespace Listonos.NavigationSystem.Tests
{
  public enum NavigationScreen
  {
    A,
    B,
  }

  public class NavigationScreenFilter : NavigationFilter<NavigationScreen>
  {
  };

  public class NavigationSystemTests
  {
    [Test]
    public void EnumAndIntInstance()
    {
      Assert.NotNull(NavigationSystem<int>.Instance);
      Assert.NotNull(NavigationSystem<NavigationScreen>.Instance);
    }

    [Test]
    public void ScreenChangedEvent()
    {
      var instance = NavigationSystem<NavigationScreen>.Instance;
      instance.CurrentScreen = NavigationScreen.A;
      EventHandler<NavigationSystem<NavigationScreen>.ScreenChangedEventArgs> listener = (sender, eventArgs) =>
      {
        Assert.AreEqual(eventArgs.NewScreen, NavigationScreen.B);
        Assert.AreEqual(sender, instance);
      };

      instance.ScreenChanged += listener;

      instance.CurrentScreen = NavigationScreen.B;

      instance.ScreenChanged -= listener;

      instance.CurrentScreen = NavigationScreen.A;
    }

    [Test]
    public void NavigationFilter()
    {
      var instance = NavigationSystem<NavigationScreen>.Instance;
      instance.CurrentScreen = NavigationScreen.A;

      var screenAGO = new GameObject("ScreenA");
      var screenAFilter = screenAGO.AddComponent<NavigationScreenFilter>();
      screenAFilter.ActiveOnScreens = new[] { NavigationScreen.A };

      var screenBGO = new GameObject("ScreenB");
      var screenBFilter = screenBGO.AddComponent<NavigationScreenFilter>();
      screenBFilter.ActiveOnScreens = new[] { NavigationScreen.B };

      var sharedScreenGO = new GameObject("SharedScreen");
      var sharedScreenFilter = sharedScreenGO.AddComponent<NavigationScreenFilter>();
      sharedScreenFilter.ActiveOnScreens = new[] { NavigationScreen.A, NavigationScreen.B };

      screenAFilter.Start();
      screenBFilter.Start();
      sharedScreenFilter.Start();

      Assert.IsTrue(screenAGO.activeSelf);
      Assert.IsFalse(screenBGO.activeSelf);
      Assert.IsTrue(sharedScreenGO.activeSelf);

      instance.CurrentScreen = NavigationScreen.B;

      Assert.IsFalse(screenAGO.activeSelf);
      Assert.IsTrue(screenBGO.activeSelf);
      Assert.IsTrue(sharedScreenGO.activeSelf);
    }
  }
};
