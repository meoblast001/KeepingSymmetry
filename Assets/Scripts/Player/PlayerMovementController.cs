﻿using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

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

  [Inject(Id = PlayerType.Player1)] private SceneGridActor player1;
  [Inject(Id = PlayerType.Player2)] private SceneGridActor player2;

  private ActivePlayerIndicator player1ActiveIndicator;
  private ActivePlayerIndicator player2ActiveIndicator;

  public LevelModels.SymmetryAxis SymmetryAxis { get; set; }

  public void Start() {
    this.player1ActiveIndicator = player1.GetComponent<ActivePlayerIndicator>();
    this.player2ActiveIndicator = player2.GetComponent<ActivePlayerIndicator>();
    this.SetActivePlayerIndicator(this.activePlayer);
  }

  public void OnMove(InputValue value) {
    var inputVec = value.Get<Vector2>();
    var rotatedMovementDirection = this.CameraRelativeMovement(DirectionFromInput(inputVec));
    Debug.Log("Move " + inputVec + " -> " + rotatedMovementDirection);
    switch (this.activePlayer) {
      case ActivePlayer.Player1: {
        MovementDirection player1Dir = rotatedMovementDirection;
        MovementDirection player2Dir = this.SymmetricalMovement(rotatedMovementDirection);
        if (this.player1.CanMove(player1Dir) && this.player2.CanMove(player2Dir)) {
          this.player1.MoveAdjacentUnsynced(player1Dir);
          this.player2.MoveAdjacentUnsynced(player2Dir);
        }
        break;
      }
      case ActivePlayer.Player2: {
        MovementDirection player1Dir = this.SymmetricalMovement(rotatedMovementDirection);
        MovementDirection player2Dir = rotatedMovementDirection;
        if (this.player1.CanMove(player1Dir) && this.player2.CanMove(player2Dir)) {
          this.player1.MoveAdjacentUnsynced(player1Dir);
          this.player2.MoveAdjacentUnsynced(player2Dir);
        }
        break;
      }
    }
  }

  public void OnToggleActive(InputValue value) {
    this.activePlayer = activePlayer == ActivePlayer.Player1 ? ActivePlayer.Player2 : ActivePlayer.Player1;
    this.SetActivePlayerIndicator(this.activePlayer);
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
    switch (this.SymmetryAxis) {
      case LevelModels.SymmetryAxis.AxisX:
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
      case LevelModels.SymmetryAxis.AxisY:
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
      case LevelModels.SymmetryAxis.AxisXY:
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

  private void SetActivePlayerIndicator(ActivePlayer activePlayer) {
    if (this.player1ActiveIndicator == null || this.player2ActiveIndicator == null) {
      Debug.LogWarning("Active player indicator missing");
      return;
    }

    switch (activePlayer) {
      case ActivePlayer.Player1:
        this.player1ActiveIndicator.SetActive(true);
        this.player2ActiveIndicator.SetActive(false);
        break;
      case ActivePlayer.Player2:
        this.player1ActiveIndicator.SetActive(false);
        this.player2ActiveIndicator.SetActive(true);
        break;
    }
  }
}
