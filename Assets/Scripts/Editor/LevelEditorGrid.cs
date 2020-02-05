using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents the level grid for the level editor. Used for reading and writing grid data.
/// </summary>
public class LevelEditorGrid<Item>
  where Item : struct
{
  private Tuple<int, int> size;
  private List<List<Item?>> grid;

  public LevelEditorGrid(int width, int height) {
    this.Reallocate(width, height);
  }

  /// <summary>
  /// Change dimensions of grid. If size reduced, items in those cells are discarded. If size increased, new cells are
  /// empty.
  /// </summary>
  public void Reallocate(int width, int height) {
    if (this.size != null && this.size.Item1 == width && this.size.Item2 == height)
      return;

    var currentGrid = this.grid;

    this.size = new Tuple<int, int>(width, height);
    this.grid = Enumerable.Range(0, height)
      .Select(y => new List<Item?>(Enumerable.Range(0, width).Select<int, Item?>(x => TryGetInGrid(currentGrid, x, y))))
      .ToList();
  }

  /// <summary>
  /// Sets or discards the item at given coordinates.
  /// </summary>
  public void SetItem(int x, int y, Item? item) {
    if (x < this.size.Item1 && y < this.size.Item2)
      this.grid[y][x] = item;
  }

  /// <summary>
  /// Gets item at given coordinates or null if none.
  /// </summary>
  public Item? GetItem(int x, int y) {
    return (x < this.size.Item1 && y < this.size.Item2) ? this.grid[y][x] : null;
  }

  /// <summary>
  /// Flattens the contents of the grid to a linear collection of cells in row-major order.
  /// </summary>
  public ICollection<Item?> ToFlatGrid() {
    return grid.SelectMany(row => row).ToList();
  }

  /// <summary>
  /// Creates an instance from a collection of cells in row-major order.
  /// </summary>
  public static LevelEditorGrid<Item> FromFlatGrid(Item?[] flatGrid, int width, int height) {
    var grid = new LevelEditorGrid<Item>(width, height);
    for (var y = 0; y < height; ++y) {
      for (var x = 0; x < width; ++x) {
        grid.grid[y][x] = flatGrid[y * width + x];
      }
    }
    return grid;
  }

  /// <summary>
  /// Gets the cell at given coordinates. If the coordinates do not exist, or no item is at the cell, null is returned.
  /// </summary>
  private static Item? TryGetInGrid(List<List<Item?>> grid, int x, int y) {
    return (grid != null && y < grid.Count && x < grid[y].Count) ? grid[y][x] : null;
  }
}
