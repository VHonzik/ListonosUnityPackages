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

  public class NavigationScreenSystem : NavigationSystem<NavigationScreen>
  {
  };

  public class NavigationSystemTests
  {
    [Test]
    public void EnumAndIntInstance()
    {
      var intInstanceGO = new GameObject();
      var intInstance = intInstanceGO.AddComponent<NavigationSystemInt>();
      Assert.NotNull(intInstance);
      intInstance.InitialScreen = 1;
      intInstance.Start();
      Assert.AreEqual(intInstance.CurrentScreen, 1);

      var enumInstanceGO = new GameObject();
      var enumInstance = enumInstanceGO.AddComponent<NavigationScreenSystem>();
      Assert.NotNull(enumInstance);
      enumInstance.InitialScreen = NavigationScreen.B;
      enumInstance.Start();
      Assert.AreEqual(enumInstance.CurrentScreen, NavigationScreen.B);
    }

    [Test]
    public void ScreenChangedEvent()
    {
      var instanceGO = new GameObject();
      var instance = instanceGO.AddComponent<NavigationScreenSystem>();

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
    public void NavigationFilterSystemFind()
    {
      var instanceGO = new GameObject();
      var instance = instanceGO.AddComponent<NavigationScreenSystem>();

      var screenGO = new GameObject("Screen");
      screenGO.transform.SetParent(instanceGO.transform);
      var screenAFilter = screenGO.AddComponent<NavigationScreenFilter>();
      screenAFilter.Awake();
      Assert.AreEqual(screenAFilter.NavigationSystem, instance);
    }

    [Test]
    public void NavigationFilter()
    {
      var instanceGO = new GameObject();
      var instance = instanceGO.AddComponent<NavigationScreenSystem>();
      instance.CurrentScreen = NavigationScreen.A;

      var screenAGO = new GameObject("ScreenA");
      var screenAFilter = screenAGO.AddComponent<NavigationScreenFilter>();
      screenAFilter.NavigationSystem = instance;
      screenAFilter.ActiveOnScreens = new[] { NavigationScreen.A };

      var screenBGO = new GameObject("ScreenB");
      var screenBFilter = screenBGO.AddComponent<NavigationScreenFilter>();
      screenBFilter.NavigationSystem = instance;
      screenBFilter.ActiveOnScreens = new[] { NavigationScreen.B };

      var sharedScreenGO = new GameObject("SharedScreen");
      var sharedScreenFilter = sharedScreenGO.AddComponent<NavigationScreenFilter>();
      sharedScreenFilter.NavigationSystem = instance;
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
