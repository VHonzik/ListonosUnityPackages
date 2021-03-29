using System;
using UnityEngine;

namespace Listonos.NavigationSystem
{
  public class NavigationSystem<T> : Singleton<NavigationSystem<T>> where T : Enum
  {
    public class ScreenChangedEventArgs : EventArgs
    {
      public T NewScreen { get; set; }
    }

    private T currentScreen;
    public T CurrentScreen
    {
      get
      {
        return currentScreen;
      }

      set
      {
        if (!value.Equals(currentScreen))
        {
          Debug.Log(string.Format("Changed nav screen to: {0}", value));
          currentScreen = value;
          ScreenChanged?.Invoke(this, new ScreenChangedEventArgs() { NewScreen = currentScreen });
        }
      }
    }

    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;
  }
}