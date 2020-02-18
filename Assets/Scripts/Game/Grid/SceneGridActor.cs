using UnityEngine;
using Zenject;

/// <summary>
/// Inserts a game object into the scene grid and enables movement within the grid. Game object position is synchronised
/// with position in scene grid.
/// </summary>
public class SceneGridActor : MonoBehaviour {
  [Inject] private SceneGrid sceneGrid = null;

  [SerializeField] private Point currentPoint;

  void OnEnable() {
    this.transform.position
      = this.sceneGrid.SetActorPoint(this, this.currentPoint);
  }

  void OnDisable() {
    this.sceneGrid.UnsetActor(this);
  }

  public void SetGridPoint(Point point) {
    this.currentPoint = point;
    if (this.enabled) {
      this.transform.position = this.sceneGrid.SetActorPoint(this, this.currentPoint);
    }
  }
}
