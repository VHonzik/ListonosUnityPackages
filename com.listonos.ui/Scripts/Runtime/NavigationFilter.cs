using System.Linq;
using UnityEngine;

namespace Listonos.NavigationSystem
{
  public class NavigationFilter<T> : MonoBehaviour where T : struct
  {
    public T[] ActiveOnScreens;

    // Start is called before the first frame update
    public void Start()
    {
      var navigationSystem = NavigationSystem<T>.Instance;
      if (!ActiveOnScreens.Contains(navigationSystem.CurrentScreen))
      {
        gameObject.SetActive(false);
      }
      navigationSystem.ScreenChanged += Navigation_ScreenChanged;
    }

    private void Navigation_ScreenChanged(object sender, NavigationSystem<T>.ScreenChangedEventArgs e)
    {
      gameObject.SetActive(ActiveOnScreens.Contains(e.NewScreen));
    }

  }
}
