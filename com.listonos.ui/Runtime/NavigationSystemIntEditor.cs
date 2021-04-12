using UnityEditor;

namespace Listonos.NavigationSystem
{
  [CustomEditor(typeof(NavigationSystemInt))]
  [CanEditMultipleObjects]
  public class NavigationSystemIntEditor : NavigationSystemEditor<int>
  {
    public override int RenderValue()
    {
      return EditorGUILayout.IntField(value);
    }
  }
}
