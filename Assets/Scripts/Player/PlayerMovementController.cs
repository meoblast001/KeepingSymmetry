using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour {
  public enum ActivePlayer {
    Player1,
    Player2
  }

  public ActivePlayer activePlayer = ActivePlayer.Player1;

  public void OnMove(InputValue value) {
    Debug.Log("Move " + value.Get<Vector2>());
  }

  public void OnToggleActive(InputValue value) {
    Debug.Log("Toggle");
  }

  public void OnRotateCamera(InputValue value) {
    Debug.Log("Rotate " + value.Get<float>());
  }
}
