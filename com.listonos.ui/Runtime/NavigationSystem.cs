using System;
using UnityEngine;

namespace Listonos.NavigationSystem
{
  public abstract class NavigationSystem<T> : MonoBehaviour
  {
    public T InitialScreen;

    public virtual void Start()
    {
      CurrentScreen = InitialScreen;
    }

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
          currentScreen = value;
          ScreenChanged?.Invoke(this, new ScreenChangedEventArgs() { NewScreen = currentScreen });
        }
      }
    }

    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;
  }
}