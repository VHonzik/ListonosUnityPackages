# Listonos' navigation system

## Usage

In order for user to be able to provide their own `enum` for navigation screens, the navigation system is using C# generic classes. Changing the current screen is easy, simply call the singleton passing your own enum type as generic argument:

```C#
using Listonos.NavigationSystem;

// Somwhere in the main assembly define enum with navigation screens
public enum NavigationScreen
{
  MainMenu,
  Credits
}

// Anywhere in code when you need to change the current navigation screen, such when a menu button is pressed, call NavigationSystem singleton
public class GameManager : MonoBehaviour
{
  // Connected to an UI button On Click event
  public void MainMenuButtonPressed()
  {
    NavigationSystem<NavigationScreen>.Instance.CurrentScreen = NavigationScreen.MainMenu;
  }
}
```

To use the provided `NavigationFilter` behavior with your own enum you will need to derived a class from it:

```C#
using Listonos.NavigationSystem;

public class NavigationScreenFilter : NavigationFilter<NavigationScreen>
{
}
```

Then simply add this script to a parent game object which houses all of the particular screen UI elements and in Editor add enum values to `Active On Screens` array of this behavior. This object will then be activated or disabled based on the navigation system's current screen via `GameObject.SetActive()`.