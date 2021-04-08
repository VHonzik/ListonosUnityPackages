using System;
using UnityEditor;
using UnityEngine;


namespace Listonos.TimeSystem
{
  [CustomPropertyDrawer(typeof(DateTimeTicksWrapper))]
  public class DateTimeTicksWrapperDrawer : PropertyDrawer 
  {
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Using BeginProperty / EndProperty on the parent property means that
      // prefab override logic works on the entire property.
      EditorGUI.BeginProperty(position, label, property);

      // Draw label
      position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

      // Don't make child fields be indented
      var indent = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;

      var ticksProperty = property.FindPropertyRelative("Ticks");
      var currentTicks = ticksProperty.longValue; 
      var currentDateTime = new DateTime(currentTicks);
      var year = currentDateTime.Year;
      var month = currentDateTime.Month;
      var day = currentDateTime.Day;
      var hour = currentDateTime.Hour;
      var minute = currentDateTime.Minute;
      var second = currentDateTime.Second;

      // Calculate rects
      var ticksRect = new Rect(position.x, position.y, 200, 20);
      var yearRect = new Rect(40, 50, 50, 20);
      var monthRect = new Rect(100, 50, 30, 20);
      var dayRect = new Rect(140, 50, 30, 20);
      var hoursRect = new Rect(180, 50, 30, 20);
      var minutesRect = new Rect(220, 50, 30, 20);
      var secondsRect = new Rect(260, 50, 30, 20);

      // Draw UI
      EditorGUI.LabelField(ticksRect, currentTicks.ToString());
      year = EditorGUI.IntField(yearRect, GUIContent.none, year);
      month = EditorGUI.IntField(monthRect, GUIContent.none, month);
      day = EditorGUI.IntField(dayRect, GUIContent.none, day);
      hour = EditorGUI.IntField(hoursRect, GUIContent.none, hour);
      minute = EditorGUI.IntField(minutesRect, GUIContent.none, minute);
      second = EditorGUI.IntField(secondsRect, GUIContent.none, second);

      if (year != currentDateTime.Year)
      {
        year = Mathf.Clamp(year, 1, 9999);
        var newDateTime = currentDateTime.AddYears(year - currentDateTime.Year);
        ticksProperty.longValue = newDateTime.Ticks;
      }

      if (month != currentDateTime.Month)
      {
        month = Mathf.Clamp(month, 1, 12);
        var newDateTime = currentDateTime.AddMonths(month - currentDateTime.Month);
        ticksProperty.longValue = newDateTime.Ticks;
      }

      if (day != currentDateTime.Day)
      {
        day = Mathf.Clamp(day, 1, 31);
        var newDateTime = currentDateTime.AddDays(day - currentDateTime.Day);
        ticksProperty.longValue = newDateTime.Ticks;
      }

      if (hour != currentDateTime.Hour)
      {
        hour = Mathf.Clamp(hour, 0, 24);
        var newDateTime = currentDateTime.AddHours(hour - currentDateTime.Hour);
        ticksProperty.longValue = newDateTime.Ticks;
      }
      
      if (minute != currentDateTime.Minute)
      {
        minute = Mathf.Clamp(minute, 0, 60);
        var newDateTime = currentDateTime.AddMinutes(minute - currentDateTime.Minute);
        ticksProperty.longValue = newDateTime.Ticks;
      }
      
      if (second != currentDateTime.Second)
      {
        second = Mathf.Clamp(second, 0, 60);
        var newDateTime = currentDateTime.AddSeconds(second - currentDateTime.Second);
        ticksProperty.longValue = newDateTime.Ticks;
      }

      // Set indent back to what it was
      EditorGUI.indentLevel = indent;

      EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property,
                                        GUIContent label)
    {
      return base.GetPropertyHeight(property, label) * 3;
    }
  }
}

