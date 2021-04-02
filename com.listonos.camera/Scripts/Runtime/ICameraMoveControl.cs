using System;

namespace Listonos.Camera
{
  [Flags]
  public enum MoveDirection
  {
    None = 1 << 0,
    Left = 1 << 1,
    Right = 1 << 2,
    Up = 1 << 3,
    Down = 1 << 4
  }

  public class MoveDirectionChangedEventArgs : EventArgs
  {
    public MoveDirection WantedDirection { get; set; }
  }

  public interface ICameraMoveControl
  {
    public event EventHandler<MoveDirectionChangedEventArgs> WantedMoveDirectionChanged;
  }

}