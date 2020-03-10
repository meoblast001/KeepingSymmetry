using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour {
  public enum ActivePlayer {
    Player1,
    Player2
  }

  public enum CameraDirection {
    ZForward,
    XForward,
    ZBackward,
    XBackward
  }

  public ActivePlayer activePlayer = ActivePlayer.Player1;
  public CameraDirection cameraDirection = CameraDirection.ZForward;

  [SerializeField] private SceneGridActor player1;
  [SerializeField] private SceneGridActor player2;
  [SerializeField] private SymmetryAxis symmetryAxis;

  public void OnMove(InputValue value) {
    var inputVec = value.Get<Vector2>();
    var rotatedMovementDirection = this.CameraRelativeMovement(DirectionFromInput(inputVec));
    Debug.Log("Move " + inputVec + " -> " + rotatedMovementDirection);
    switch (this.activePlayer) {
      case ActivePlayer.Player1:
        this.player1.MoveAdjacentUnsynced(rotatedMovementDirection);
        this.player2.MoveAdjacentUnsynced(this.SymmetricalMovement(rotatedMovementDirection));
        break;
      case ActivePlayer.Player2:
        this.player1.MoveAdjacentUnsynced(this.SymmetricalMovement(rotatedMovementDirection));
        this.player2.MoveAdjacentUnsynced(rotatedMovementDirection);
        break;
    }
  }

  public void OnToggleActive(InputValue value) {
    this.activePlayer = activePlayer == ActivePlayer.Player1 ? ActivePlayer.Player2 : ActivePlayer.Player1;
    Debug.Log("Active Player is " + this.activePlayer);
  }

  public void OnRotateCamera(InputValue value) {
    var newCameraDirection = value.Get<float>() > 0
      ? RotatedCameraDirectionRight(this.cameraDirection)
      : RotatedCameraDirectionLeft(this.cameraDirection);
    this.cameraDirection = newCameraDirection;
    Debug.Log("New Camera Direction " + this.cameraDirection);
  }

  private static CameraDirection RotatedCameraDirectionRight(CameraDirection direction) {
    switch (direction) {
      case CameraDirection.ZForward: return CameraDirection.XForward;
      case CameraDirection.XForward: return CameraDirection.ZBackward;
      case CameraDirection.ZBackward: return CameraDirection.XBackward;
      case CameraDirection.XBackward: return CameraDirection.ZForward;
      default:
        Debug.LogError("Invalid camera direction: " + direction);
        return CameraDirection.ZForward;
    }
  }

  private static CameraDirection RotatedCameraDirectionLeft(CameraDirection direction) {
    switch (direction) {
      case CameraDirection.ZForward: return CameraDirection.XBackward;
      case CameraDirection.XForward: return CameraDirection.ZForward;
      case CameraDirection.ZBackward: return CameraDirection.XForward;
      case CameraDirection.XBackward: return CameraDirection.ZBackward;
      default:
        Debug.LogError("Invalid camera direction: " + direction);
        return CameraDirection.ZForward;
    }
  }

  private static MovementDirection DirectionFromInput(Vector2 inputVec) {
    if (inputVec.x > 0f)
      return MovementDirection.Right;
    else if (inputVec.x < 0f)
      return MovementDirection.Left;
    else if (inputVec.y > 0f)
      return MovementDirection.Forward;
    else if (inputVec.y < 0f)
      return MovementDirection.Backward;
    else
      return MovementDirection.None;
  }

  private MovementDirection CameraRelativeMovement(MovementDirection direction) {
    switch (this.cameraDirection) {
      case CameraDirection.ZForward:
        return direction;
      case CameraDirection.ZBackward:
        switch (direction) {
          case MovementDirection.Left:
            return MovementDirection.Right;
          case MovementDirection.Forward:
            return MovementDirection.Backward;
          case MovementDirection.Right:
            return MovementDirection.Left;
          case MovementDirection.Backward:
            return MovementDirection.Forward;
        }
        break;
      case CameraDirection.XForward:
        switch (direction) {
          case MovementDirection.Left:
            return MovementDirection.Forward;
          case MovementDirection.Forward:
            return MovementDirection.Right;
          case MovementDirection.Right:
            return MovementDirection.Backward;
          case MovementDirection.Backward:
            return MovementDirection.Left;
        }
        break;
      case CameraDirection.XBackward:
        switch (direction) {
          case MovementDirection.Left:
            return MovementDirection.Backward;
          case MovementDirection.Forward:
            return MovementDirection.Left;
          case MovementDirection.Right:
            return MovementDirection.Forward;
          case MovementDirection.Backward:
            return MovementDirection.Right;
        }
        break;
      default:
        Debug.LogError("Invalid camera direction: " + direction);
        return MovementDirection.None;
    }
    Debug.Log("Invalid movement direciton: " + direction);
    return MovementDirection.None;
  }

  private MovementDirection SymmetricalMovement(MovementDirection direction) {
    switch (this.symmetryAxis) {
      case SymmetryAxis.AxisX:
        switch (direction) {
          case MovementDirection.Forward:
            return MovementDirection.Forward;
          case MovementDirection.Right:
            return MovementDirection.Left;
          case MovementDirection.Backward:
            return MovementDirection.Backward;
          case MovementDirection.Left:
            return MovementDirection.Right;
        }
        break;
      case SymmetryAxis.AxisZ:
        switch (direction) {
          case MovementDirection.Forward:
            return MovementDirection.Backward;
          case MovementDirection.Right:
            return MovementDirection.Right;
          case MovementDirection.Backward:
            return MovementDirection.Forward;
          case MovementDirection.Left:
            return MovementDirection.Left;
        }
        break;
      case SymmetryAxis.AxisXZ:
        switch (direction) {
          case MovementDirection.Forward:
            return MovementDirection.Backward;
          case MovementDirection.Right:
            return MovementDirection.Left;
          case MovementDirection.Backward:
            return MovementDirection.Forward;
          case MovementDirection.Left:
            return MovementDirection.Right;
        }
        break;
    }

    Debug.LogWarning("Unknown inversion of movement direction: " + direction);
    return MovementDirection.None;
  }
}
