using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Listonos.NavigationSystem
{
  public abstract class NavigationFilter<T> : MonoBehaviour
  {
    [Tooltip("Navigation System this filter belongs to. If null, GetComponentInParent<> will be used.")]
    public NavigationSystem<T> NavigationSystem;
    public T[] ActiveOnScreens;

    protected IEqualityComparer<T> equalityComparer { get; set; } = EqualityComparer<T>.Default;

    public virtual void Awake()
    {
      if (NavigationSystem == null)
      {
        NavigationSystem = GetComponentInParent<NavigationSystem<T>>();
        Debug.AssertFormat(NavigationSystem != null, "NavigationFilter<{0}> did not find appropriate NavigationSystem in the hierarchy and will not work!", typeof(T).Name);
      }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
      if (!ActiveOnScreens.Contains(NavigationSystem.CurrentScreen))
      {
        gameObject.SetActive(false);
      }
      NavigationSystem.ScreenChanged += Navigation_ScreenChanged;
    }

    public bool IsScreenPartOfActiveArray(T screen)
    {
      for (int i = 0; i < ActiveOnScreens.Length; i++)
      {
        if (equalityComparer.Equals(ActiveOnScreens[i], screen))
        {
          return true;
        }
      }
      return false;
    }

    private void Navigation_ScreenChanged(object sender, NavigationSystem<T>.ScreenChangedEventArgs e)
    {
      gameObject.SetActive(IsScreenPartOfActiveArray(e.NewScreen));
    }

  }
}
