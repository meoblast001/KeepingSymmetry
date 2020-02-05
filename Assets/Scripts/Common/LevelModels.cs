using System.Xml.Serialization;

public static class LevelModels {
  [XmlRootAttribute("Level")]
  public class Level {
    public int Width { get; set; }
    public int Height { get; set; }
    public SymmetryAxis SymmetryAxis { get; set; }
    public Grid Grid { get; set; }
  }

  public class Grid {
    [XmlArrayItem("Item", Type = typeof(GridItem))]
    [XmlArrayItem("Empty", Type = typeof(EmptyItem))]
    public AbstractGridItem[] GridItems { get; set; }
  }

  [XmlInclude(typeof(GridItem))]
  [XmlInclude(typeof(EmptyItem))]
  public abstract class AbstractGridItem {
    // Intentionally empty.
  }

  public class GridItem : AbstractGridItem {
    [XmlAttribute]
    public GridItemType Type { get; set; }
  }

  public class EmptyItem : AbstractGridItem {
    // Intentionally empty.
  }

  public enum SymmetryAxis {
    AxisX,
    AxisY
  }

  public enum GridItemType {
    Wall0Edge0,
    Wall1Edge0, // Edge on left
    Wall1Edge1, // Edge on top
    Wall1Edge2, // Edge on right
    Wall1Edge3, // Edge on bottom
    Wall2Edge0, // Edges on bottom and right
    Wall2Edge1, // Edges on bottom and left
    Wall2Edge2, // Edges on top and left
    Wall2Edge3, // Edges on top and right
    Wall2Edge4, // Edges on top and bottom
    Wall2Edge5, // Edges on left and right
    Wall3Edge0, // Opening on left
    Wall3Edge1, // Opening on top
    Wall3Edge2, // Opening on right
    Wall3Edge3, // Opening on bottom
    Wall4Edge0,
    Player0,
    Player1,
    Goal
  }
}
