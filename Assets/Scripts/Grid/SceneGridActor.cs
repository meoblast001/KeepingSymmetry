using UnityEngine;
using Zenject;

/// <summary>
/// Inserts a game object into the scene grid and enables movement within the grid. Game object position is synchronised
/// with position in scene grid.
/// </summary>
public class SceneGridActor : MonoBehaviour, SceneGrid.IMoveCallback {
  [Inject] private SceneGrid sceneGrid = null;

  [SerializeField] private Point currentPoint;
  [SerializeField] private bool isSolid;

  private object uniqueId;

  void Awake() {
    this.uniqueId = this.gameObject.GetInstanceID();
  }

  void OnEnable() {
    this.sceneGrid.SetIsSolid(this.uniqueId, this.isSolid);
    this.transform.position
      = this.sceneGrid.SetActorPoint(this.uniqueId, this.currentPoint);
  }

  void OnDisable() {
    this.sceneGrid.UnsetActor(this.uniqueId);
    this.sceneGrid.UnsetIsSolid(this.uniqueId);
  }

  public void SetGridPoint(Point point) {
    this.currentPoint = point;
    if (this.enabled) {
      this.ApplyPosition(this.sceneGrid.SetActorPoint(this.uniqueId, this.currentPoint));
    }
  }

  public bool CanMove(MovementDirection direction) {
    return this.sceneGrid.CanMove(this.uniqueId, direction);
  }

  public bool MoveAdjacentUnsynced(MovementDirection direction) {
    return this.sceneGrid.UnsyncedMoveAdjacent(this.uniqueId, direction, this);
  }

  public bool MoveAdjacentSynced(MovementDirection direction) {
    return this.sceneGrid.SyncedMoveAdjacent(this.uniqueId, direction, this);
  }

  public void OnMoveUpdated(Vector3 position) {
    this.ApplyPosition(position);
  }

  public void OnMoveCompleted(Point occupiedPoint) {
    this.currentPoint = occupiedPoint;
  }

  private void ApplyPosition(Vector3 sceneGridPos) {
    this.transform.position = new Vector3(sceneGridPos.x, sceneGridPos.y, sceneGridPos.z);
  }
}
