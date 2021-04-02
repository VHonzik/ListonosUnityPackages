using System;
using System.Linq;
using UnityEngine;

namespace Listonos.Camera
{
  public class TopDownCamera : MonoBehaviour
  {
    private MoveDirection moveDirection = MoveDirection.None;
    private ICameraMoveControl moveDirectionSource;
    public float FinalMoveSpeed;
    public float StartingMoveSpeed;
    public AnimationCurve MoveAccelerationCurve;
    public float MoveDeaccelerationSpeed;

    private float moveSpeed;
    private float movementTimer;
    private Vector3 worldMoveDirection;
    private Vector3 inertiaWorldMoveDirection;

    void Start()
    {
      var movementControls = GetComponents<MonoBehaviour>().OfType<ICameraMoveControl>();
      Debug.AssertFormat(movementControls.Count() != 0, "TopDownCamera behavior did not find any IMovementControl behaviors and therefore camera won't move.");
      foreach (var movementControl in movementControls)
      {
        movementControl.WantedMoveDirectionChanged += MovementControl_WantedMoveDirectionChanged;
      }
    }

    private void MovementControl_WantedMoveDirectionChanged(object sender, MoveDirectionChangedEventArgs e)
    {
      // No control is telling us to move yet but the sender just changed to move
      if (e.WantedDirection != MoveDirection.None && moveDirectionSource == null)
      {
        moveDirectionSource = sender as ICameraMoveControl;
        moveDirection = e.WantedDirection;
        RecalculateWorldMoveDirection(moveDirection);
      }
      // Active source stopped moving
      else if (moveDirectionSource == sender && e.WantedDirection != moveDirection)
      {
        moveDirection = e.WantedDirection;
        RecalculateWorldMoveDirection(moveDirection);
        if (moveDirection == MoveDirection.None)
        {
          moveDirectionSource = null;
        }
      }
    }

    void Update()
    {
      if (moveDirection != MoveDirection.None)
      {
        inertiaWorldMoveDirection = worldMoveDirection;
        transform.position += worldMoveDirection * Time.deltaTime * moveSpeed;

        movementTimer = Mathf.Clamp01(movementTimer + Time.deltaTime);
        moveSpeed = Mathf.Lerp(StartingMoveSpeed, FinalMoveSpeed, MoveAccelerationCurve.Evaluate(movementTimer));
      }
      else
      {
        if (movementTimer > 0.0f)
        {
          movementTimer = Mathf.Clamp01(movementTimer - Time.deltaTime);

          transform.position += inertiaWorldMoveDirection * Time.deltaTime * moveSpeed;
          moveSpeed = Mathf.Clamp(moveSpeed - MoveDeaccelerationSpeed * Time.deltaTime, 0, StartingMoveSpeed);

          if (movementTimer <= 0.0f)
          {            
            movementTimer = 0.0f;
            moveSpeed = StartingMoveSpeed;
          }
        }
      }
    }

    private void RecalculateWorldMoveDirection(MoveDirection moveDirection)
    {
      var rightDirection = 0.0f;
      if ((moveDirection & MoveDirection.Left) != 0) rightDirection = -1.0f;
      else if ((moveDirection & MoveDirection.Right) != 0) rightDirection = 1.0f;

      var upDirection = 0.0f;
      if ((moveDirection & MoveDirection.Up) != 0) upDirection = -1.0f;
      else if ((moveDirection & MoveDirection.Down) != 0) upDirection = 1.0f;

      var worldRightDirection = transform.right * rightDirection;
      var worldXZForwardDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * upDirection;
      worldMoveDirection = worldRightDirection + worldXZForwardDirection;
    }
  }
}