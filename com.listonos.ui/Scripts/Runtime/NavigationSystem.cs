using System;

namespace Listonos.NavigationSystem
{
  public class NavigationSystem<T> : Singleton<NavigationSystem<T>> where T : struct
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
          currentScreen = value;
          ScreenChanged?.Invoke(this, new ScreenChangedEventArgs() { NewScreen = currentScreen });
        }
      }
    }

    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;
  }
}