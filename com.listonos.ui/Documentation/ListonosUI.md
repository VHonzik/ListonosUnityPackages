# Listonos' UI

Listonos' UI is a collection of UI elements and tools that is easy to use and looks good right out of the box. It is mainly intended for jump-starting game dev jam projects.

## UI elements usage

Simply grab the prefabs from `Runtime/Assets/Prefabs` and use them somewhere in UI hierarchy under UI Canvas object.

Note that if you want to disable Button and Button-derived elements through scripts, use the `Listonos.UI.Button.Interactible` property instead of the underlying Unity `Button.interactable`.

## Navigation System usage

Navigation system is based on generic classes in order to provide user ability to set up one with user-defined enum (which therefore lives in a different assembly). There are two behaviors necessary to make Navigation System work, `NavigationSystem<>` and `NavigationFilter<>`. `Listonos.UI` package comes with ready-to-go behaviors for `int`-based Navigation System: `NavigationSystemInt` and appropriate `NavigationFilterInt`. In order to use user-defined enum you will only need couple of files with few lines of code:

```C#
// MyNavigationScreen.cs or anywhere in the main, predefined, assembly
using Listonos.NavigationSystem;

public enum MyNavigationScreen
{
  MainMenu,
  Credits
}
```

```C#
// MyNavigationFilter.cs
using Listonos.NavigationSystem;

public class MyNavigationFilter : NavigationFilter<MyNavigationScreen>
{
}
```

```C#
// MyNavigationSystem.cs
using Listonos.NavigationSystem;

public class MyNavigationSystem : NavigationSystem<MyNavigationScreen>
{
}
```

This package also provides custom editor for `NavigationSystem` that allows to quickly manipulate the hierarchy of `NavigationFilter`s. `NavigationFilterInt` has it built-in once again and for the above example you would need to add following lines to the `MyNavigationSystem.cs` if you want to use it:

```C#
[CustomEditor(typeof(MyNavigationSystem))]
[CanEditMultipleObjects]
public class MyNavigationSystemEditor : NavigationSystemEditor<MyNavigationScreen>
{
  public override MyNavigationScreen RenderValue()
  {
    return (MyNavigationScreen)EditorGUILayout.EnumPopup(value);
  }
}
```

In order to use Navigation System, you simply need a reference to it in your script and use the setter on `NavigationSystem<T>.CurrentScreen` property.

```C#
using Listonos.NavigationSystem;

// Anywhere in code when you need to change the current navigation screen, such when a menu button is pressed, use NavigationSystem.CurrentScreen
public class GameManager : MonoBehaviour
{
  public MyNavigationSystem NavigationSystem;

  // Connected to an UI button On Click event
  public void MainMenuButtonPressed()
  {
    NavigationSystem.CurrentScreen = MyNavigationScreen.MainMenu;
  }
}
```

`NavigationSystemInt` or your `NavigationSystem<>`-derived behavior need to be somewhere at the top of canvas hierarchy so that all `NavigationFilter`s are their children. For example game object with root `Canvas` behavior is a good candidate. Then create "root" game objects for each screen with `NavigationFilter` attached. This object will then be activated or disabled via `GameObject.SetActive()` depending on whether navigation system's current screen is found in `NavigationFilter<>.ActiveOnScreens` array. `NavigationFilter` optionally accept `NavigationSystem` reference in a special cases such as when they are NOT descendants of `NavigationSystem`, however note that the custom editor won't work in that case.


## AspectRatioFitterLayout usage

This behavior works very similarly to AspectRatioFitter however it plays nice with Layout Element behavior by setting the preferred width or height on it instead of setting the actual width or height of RectTransform.