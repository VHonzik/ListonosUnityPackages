using System;
using UnityEngine;

namespace Listonos.Camera
{
  public class KeyboardControl : MonoBehaviour, ICameraMoveControl
  {
    public string HorizontalInputAxisName = "Horizontal";
    public string VerticalInputAxisName = "Vertical";

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
      MoveDirection direction = MoveDirection.None;
      var horizontalInput = Input.GetAxis(HorizontalInputAxisName);
      var verticalInput = Input.GetAxis(VerticalInputAxisName);

      if (horizontalInput < 0.0f) direction |= MoveDirection.Left;
      else if (horizontalInput > 0.0f) direction |= MoveDirection.Right;

      if (verticalInput < 0.0f) direction |= MoveDirection.Up;
      else if (verticalInput > 0.0f) direction |= MoveDirection.Down;

      MoveDirection = direction;
    }

  }
}