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

  public void OnMove(InputValue value) {
    Debug.Log("Move " + value.Get<Vector2>());
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
}
