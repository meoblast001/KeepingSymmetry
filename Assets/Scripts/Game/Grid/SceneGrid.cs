using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Organises players into grid and keeps track of what grid locations are occupied and what movements are legal.
/// </summary>
public class SceneGrid {
  // TODO: Should be configurable.
  public const float GridElementSize = 1f;

  private Dictionary<SceneGridActor, Point> occupiedPoints = new Dictionary<SceneGridActor, Point>();

  public Vector3 SetActorPoint(SceneGridActor actor, Point newPoint) {
    this.occupiedPoints[actor] = newPoint;
    return new Vector3(newPoint.x * GridElementSize, 0f, newPoint.y * GridElementSize);
  }

  public void UnsetActor(SceneGridActor actor) {
    this.occupiedPoints.Remove(actor);
  }
}
