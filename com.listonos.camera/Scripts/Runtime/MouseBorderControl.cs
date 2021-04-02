using System;
using UnityEngine;

namespace Listonos.Camera
{
  public class MouseBorderControl : MonoBehaviour, ICameraMoveControl
  {
    public float BorderWidthPx;

    private MoveDirection moveDirection = MoveDirection.None;
    public MoveDirection MoveDirection
    {
      get
      {
        return moveDirection;
      }
      private set
      {
        if (value != moveDirection)
        {
          moveDirection = value;
          WantedMoveDirectionChanged?.Invoke(this, new MoveDirectionChangedEventArgs() { WantedDirection = moveDirection });
        }
      }
    }

    public event EventHandler<MoveDirectionChangedEventArgs> WantedMoveDirectionChanged;


    void Update()
    {
      var mousePosition = Input.mousePosition;
      MoveDirection = MovementWanted(mousePosition);
    }

    private MoveDirection MovementWanted(Vector2 mousePosition)
    {
      MoveDirection direction = MoveDirection.None;
      if (mousePosition.x <= BorderWidthPx) direction |= MoveDirection.Left;
      if (mousePosition.x >= Screen.width - BorderWidthPx) direction |= MoveDirection.Right;
      if (mousePosition.y <= BorderWidthPx) direction |= MoveDirection.Up;
      if (mousePosition.y >= Screen.height - BorderWidthPx) direction |= MoveDirection.Down;
      return direction;
    }
  }
}
