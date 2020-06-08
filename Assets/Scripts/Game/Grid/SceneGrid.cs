using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Organises actors into grid and keeps track of what grid locations are occupied and what movements are legal and
/// being performed.
/// </summary>
public class SceneGrid : ITickable {
  // TODO: Should be configurable.
  public const float GridElementSize = 1f;
  public const float MoveSeconds = 1f;

  public interface IMoveCallback {
    void OnMoveUpdated(Vector3 position);
    void OnMoveCompleted(Point occupiedPoint);
  }

  /// <summary>
  /// Records and queries what actors are at what grid points.
  /// </summary>
  private class OccupiedPoints {
    private Dictionary<object, Point> actorsToPoints = new Dictionary<object, Point>();
    private Dictionary<Point, object> pointsToActors = new Dictionary<Point, object>();

    public void Set(object actorId, Point point) {
      if (this.actorsToPoints.ContainsKey(actorId)) {
        this.Remove(actorId);
      }
      this.actorsToPoints[actorId] = point;
      this.pointsToActors[point] = actorId;
    }

    public void Remove(object actorId) {
      if (this.actorsToPoints.ContainsKey(actorId)) {
        Point point = this.actorsToPoints[actorId];
        this.actorsToPoints.Remove(actorId);
        this.pointsToActors.Remove(point);
      } else {
        Debug.LogError("Could not remove actor occupation from point.");
      }
    }

    public Point? GetPointByActorId(object actorId) {
      return this.actorsToPoints.ContainsKey(actorId) ? (Point?)this.actorsToPoints[actorId] : null;
    }

    public bool IsPointOccupied(Point point) {
      return this.pointsToActors.ContainsKey(point);
    }
  }

  private abstract class AbstractMovement {
    public Point start;
    public Point end;
    public IMoveCallback moveCallback;

    public AbstractMovement(Point start, Point end, IMoveCallback moveCallback) {
      this.start = start;
      this.end = end;
      this.moveCallback = moveCallback;
    }
  }

  /// <summary>
  /// Movement job that is not synced with the clock.
  /// </summary>
  private class UnsyncedMovement : AbstractMovement {
    public float elapsedSeconds = 0f;

    public UnsyncedMovement(Point start, Point end, IMoveCallback moveCallback) : base(start, end, moveCallback) {}
  }

  /// <summary>
  /// Movement job that begins and completes with all other active synced movement jobs.
  /// </summary>
  private class SyncedMovement : AbstractMovement {
    public SyncedMovement(Point start, Point end, IMoveCallback moveCallback) : base(start, end, moveCallback) {}
  }

  private OccupiedPoints occupiedPoints = new OccupiedPoints();
  private Dictionary<object, UnsyncedMovement> unsyncedMovements = new Dictionary<object, UnsyncedMovement>();
  private Dictionary<object, SyncedMovement> syncedMovements = new Dictionary<object, SyncedMovement>();
  private Dictionary<object, SyncedMovement> queuedSyncedMovements = new Dictionary<object, SyncedMovement>();

  public void Tick() {
    var time = Time.time;
    var deltaTime = Time.deltaTime;
    var lastTickTime = time - deltaTime;

    // Execute and complete unsynced movements. These movements begin without delay and each has its own progress.

    var removeIds = new Queue<object>();
    foreach (var actorId in this.unsyncedMovements.Keys) {
      var movement = this.unsyncedMovements[actorId];

      movement.elapsedSeconds += deltaTime;

      var progress = movement.elapsedSeconds / MoveSeconds;
      var position = Vector3.Lerp(movement.start.ToVector3OnXZPlane(), movement.end.ToVector3OnXZPlane(), progress);

      movement.moveCallback.OnMoveUpdated(position);
      if (progress >= 1f) {
        Debug.Log($"{nameof(SceneGrid)}: Completed unsynced movement for ID {actorId.ToString()}");
        this.OnMoveCompleted(actorId, movement);
        removeIds.Enqueue(actorId);
      }
    }
    foreach (var actorId in removeIds)
      this.unsyncedMovements.Remove(actorId);

    // If a synced movement clock cycle finishes, inform each synced movement job of completion, clear active jobs, and
    // make all waiting jobs in the next queue active.

    var movementStartTime = SyncedMovementStartTimeFromAbsoluteTime(time);
    var syncedMovementFinished = movementStartTime > SyncedMovementStartTimeFromAbsoluteTime(lastTickTime);
    if (syncedMovementFinished) {
      foreach (var actorId in this.syncedMovements.Keys) {
        var movement = this.unsyncedMovements[actorId];

        var position = movement.end.ToVector3OnXZPlane();

        movement.moveCallback.OnMoveUpdated(position);
        Debug.Log($"{nameof(SceneGrid)}: Completed synced movement for ID {actorId.ToString()}");
        this.OnMoveCompleted(actorId, movement);
      }

      this.syncedMovements = this.queuedSyncedMovements;
      this.queuedSyncedMovements = new Dictionary<object, SyncedMovement>();
    }

    // Progress all synced movements in parallel.

    var syncedElapsedSeconds = time - movementStartTime;
    var syncedProgress = Mathf.Clamp01(syncedElapsedSeconds / MoveSeconds);
    foreach (var movement in this.syncedMovements.Values) {
      var position = Vector3.Lerp(
        movement.start.ToVector3OnXZPlane(),
        movement.end.ToVector3OnXZPlane(),
        syncedProgress);

      movement.moveCallback.OnMoveUpdated(position);
    }
  }

  /// <summary>
  /// Registers an actor and occupies a point in the grid.
  /// </summary>
  public Vector3 SetActorPoint(object actorId, Point newPoint) {
    this.occupiedPoints.Set(actorId, newPoint);
    return new Vector3(newPoint.x * GridElementSize, 0f, newPoint.y * GridElementSize);
  }

  /// <summary>
  /// Unregisters an actor and frees its point in the grid.
  /// </summary>
  public void UnsetActor(object actorId) {
    this.occupiedPoints.Remove(actorId);
    this.unsyncedMovements.Remove(actorId);
    this.syncedMovements.Remove(actorId);
    this.queuedSyncedMovements.Remove(actorId);
  }

  /// <summary>
  /// Assigns a given actor to move to an adjacent point in some direction. Movement begins immediately. Callback is
  /// called with position and state information until the movement job completes.
  /// </summary>
  public bool UnsyncedMoveAdjacent(object actorId, MovementDirection direction, IMoveCallback moveCallback) {
    if (this.unsyncedMovements.ContainsKey(actorId))
      return false;

    Point? oldPoint = this.occupiedPoints.GetPointByActorId(actorId);
    if (!oldPoint.HasValue)
      return false;
    Point? newPoint = this.AdjacentMovementPoint(oldPoint.Value, direction);
    if (!newPoint.HasValue)
      return false;

    if (this.occupiedPoints.IsPointOccupied(newPoint.Value))
      return false;

    var movement = new UnsyncedMovement(oldPoint.Value, newPoint.Value, moveCallback);
    Debug.Log($"{nameof(SceneGrid)}: Adding unsynced movement for ID {actorId.ToString()}");
    this.unsyncedMovements.Add(actorId, movement);
    return true;
  }

  /// <summary>
  /// Assigns a given actor to move to an adjacent point in some direction. Movement is synchronised with clock to start
  /// and end with other synced movement jobs. Callback is called with position and state information until the movement
  /// job completes.
  /// </summary>
  public bool SyncedMoveAdjacent(object actorId, MovementDirection direction, IMoveCallback moveCallback) {
    if (this.queuedSyncedMovements.ContainsKey(actorId))
      return false;

    Point? oldPoint = this.occupiedPoints.GetPointByActorId(actorId);
    if (!oldPoint.HasValue)
      return false;
    Point? newPoint = this.AdjacentMovementPoint(oldPoint.Value, direction);
    if (!newPoint.HasValue)
      return false;

    if (this.occupiedPoints.IsPointOccupied(newPoint.Value))
      return false;

    var movement = new SyncedMovement(oldPoint.Value, newPoint.Value, moveCallback);
    Debug.Log($"{nameof(SceneGrid)}: Adding synced movement for ID {actorId.ToString()}");
    this.queuedSyncedMovements.Add(actorId, movement);
    return true;
  }

  private float SyncedMovementStartTimeFromAbsoluteTime(float time) {
    return ((int) time / MoveSeconds) * MoveSeconds;
  }

  private void OnMoveCompleted(object actorId, AbstractMovement movement) {
    this.SetActorPoint(actorId, movement.end);
    movement.moveCallback.OnMoveCompleted(movement.end);
  }

  /// <summary>
  /// Determines which point is adjacent to the given point in a given direction. Null if direction invalid.
  /// </summary>
  private Point? AdjacentMovementPoint(Point point, MovementDirection direction) {
    switch (direction) {
      case MovementDirection.Forward:
        ++point.y;
        break;
      case MovementDirection.Right:
        ++point.x;
        break;
      case MovementDirection.Backward:
        --point.y;
        break;
      case MovementDirection.Left:
        --point.x;
        break;
      default:
        return null;
    }
    return point;
  }
}
