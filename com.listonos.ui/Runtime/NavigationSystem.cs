using System;
using UnityEditor;
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

  public abstract class NavigationSystemEditor<T> : Editor
  {
    private SerializedProperty initialScreen;
    protected T value;

    void OnEnable()
    {
      initialScreen = serializedObject.FindProperty("InitialScreen");
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUILayout.PropertyField(initialScreen, new GUIContent("InitialScreen"));
      serializedObject.ApplyModifiedProperties();

      GUILayout.Label("Navigation System Tools", EditorStyles.boldLabel);

      GUILayout.BeginVertical("box");
      GUILayout.Label("NavigationFilter control", EditorStyles.largeLabel);
      GUILayout.Label("This editor tool allows to quickly hide or show NavigationFilter children of this Navigation System. Buttons below will activate/deactive the game objects that have NavigationFilter behavior attached. When working on specific screen you can use the Value field with 'Show only Value' button to quickly hide all screens but the one you are working on.", EditorStyles.helpBox);
      GUILayout.Label("The Value:");
      value = RenderValue();

      if (GUILayout.Button("Hide All"))
      {
        ShowElements(false, false);
      }

      if (GUILayout.Button("Show only Value"))
      {
        ShowElements(true, false);
      }

      if (GUILayout.Button("Show all but Value"))
      {
        ShowElements(false, true);
      }

      if (GUILayout.Button("Show all"))
      {
        ShowElements(true, true);
      }
      GUILayout.EndHorizontal();
    }

    public abstract T RenderValue();

    private void ShowElements(bool showValue, bool showOthers)
    {
      foreach (var inspectedObject in serializedObject.targetObjects)
      {
        var inspectedBehavior = inspectedObject as MonoBehaviour;
        var behaviours = inspectedBehavior.gameObject.GetComponentsInChildren<NavigationFilter<T>>(true);
        foreach (var behaviour in behaviours)
        {
          var hasValue = behaviour.IsScreenPartOfActiveArray(value);
          if (hasValue)
          {
            behaviour.gameObject.SetActive(showValue);
          }
          else
          {
            behaviour.gameObject.SetActive(showOthers);
          }
        }
      }
    }
  }
}