using UnityEngine;
using Zenject;

/// <summary>
/// Inserts a game object into the scene grid and enables movement within the grid. Game object position is synchronised
/// with position in scene grid.
/// </summary>
public class SceneGridActor : MonoBehaviour, SceneGrid.IMoveCallback {
  [Inject] private SceneGrid sceneGrid = null;

  [SerializeField] private Point currentPoint;

  private object uniqueId;

  void Awake() {
    this.uniqueId = this.gameObject.GetInstanceID();
  }

  void OnEnable() {
    this.transform.position
      = this.sceneGrid.SetActorPoint(this.uniqueId, this.currentPoint);
  }

  void OnDisable() {
    this.sceneGrid.UnsetActor(this.uniqueId);
  }

  public void SetGridPoint(Point point) {
    this.currentPoint = point;
    if (this.enabled) {
      this.transform.position = this.sceneGrid.SetActorPoint(this.uniqueId, this.currentPoint);
    }
  }

  public bool MoveAdjacentUnsynced(MovementDirection direction) {
    return this.sceneGrid.UnsyncedMoveAdjacent(this.uniqueId, direction, this);
  }

  public bool MoveAdjacentSynced(MovementDirection direction) {
    return this.sceneGrid.SyncedMoveAdjacent(this.uniqueId, direction, this);
  }

  public void OnMoveUpdated(Vector3 position) {
    this.transform.position = position;
  }

  public void OnMoveCompleted(Point occupiedPoint) {
    this.currentPoint = occupiedPoint;
  }
}
