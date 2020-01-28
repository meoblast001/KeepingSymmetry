using System;
using System.Collections.Generic;
using System.Linq;

public class LevelEditorGrid<Item>
  where Item : struct
{
  private Tuple<int, int> size;
  private List<List<Item?>> grid;

  public LevelEditorGrid(int width, int height) {
    this.Reallocate(width, height);
  }

  public void Reallocate(int width, int height) {
    if (this.size != null && this.size.Item1 == width && this.size.Item2 == height)
      return;

    var currentGrid = this.grid;

    this.size = new Tuple<int, int>(width, height);
    this.grid = Enumerable.Range(0, height)
      .Select(y => new List<Item?>(Enumerable.Range(0, width).Select<int, Item?>(x => TryGetInGrid(currentGrid, x, y))))
      .ToList();
  }

  public void SetItem(int x, int y, Item? item) {
    if (x < this.size.Item1 && y < this.size.Item2)
      this.grid[y][x] = item;
  }

  public Item? GetItem(int x, int y) {
    return (x < this.size.Item1 && y < this.size.Item2) ? this.grid[y][x] : null;
  }

  public ICollection<Item?> ToFlatGrid() {
    return grid.SelectMany(row => row).ToList();
  }

  private static Item? TryGetInGrid(List<List<Item?>> grid, int x, int y) {
    return (grid != null && y < grid.Count && x < grid[y].Count) ? grid[y][x] : null;
  }
}
