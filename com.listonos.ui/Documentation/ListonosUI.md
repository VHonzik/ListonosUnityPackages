# Listonos' UI

Listonos' UI is a collection of UI elements and tools that is easy to use and looks good right out of the box. It is mainly inteded for jumpstarting game dev jam projects.

## UI elements usage

Simply grab the prefabs from `Assets/Prefabs` and use them somewhere in UI hierarchy under UI Canvas object.

Note that if you want to disable Button and Button-derived elements through scripts, use the `Listonos.Buttons.Button.Interactible` property instead of the underlying Unity `Button.interactable`.

## Navigation System usage

Because we wnated users to provide their own `enum` for navigation screens for ease of use, this enum must lives in a different assembly. The navigation system is therefore using C# generic classes. Changing the current screen is easy, simply call the singleton passing your own enum type as generic argument:

```C#
using Listonos.NavigationSystem;

// Somwhere in the main, predefined assembly, define enum with navigation screens
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

To use the provided `NavigationFilter` behavior with your own enum you will need to derive a class from it:

```C#
using Listonos.NavigationSystem;

public class NavigationScreenFilter : NavigationFilter<NavigationScreen>
{
}
```

Then simply add this script to a parent game object which houses all of the particular screen UI elements and in Editor add enum values to `Active On Screens` array of this behavior. This object will then be activated or disabled based on the navigation system's current screen via `GameObject.SetActive()`.