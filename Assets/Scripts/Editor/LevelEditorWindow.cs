using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow {
  const int StartWidth = 10;
  const int StartHeight= 10;
  const int SectionSpace = 50;
  const int SymmetryLineSpace = 15;

  public struct GridSize {
    public int Width { get; set; }
    public int Height { get; set; }
  }

  public enum SymmetryAxis {
    AxisX,
    AxisY
  }

  private Vector2 scrollPosition = Vector2.zero;
  private GridSize gridSize = new GridSize() { Width = StartWidth, Height = StartHeight };
  private SymmetryAxis symmetryAxis = SymmetryAxis.AxisX;
  private LevelModels.GridItemType? currentPaletteItem;
  private LevelEditorGrid<LevelModels.GridItemType> levelGrid
    = new LevelEditorGrid<LevelModels.GridItemType>(StartWidth, StartHeight);

  [MenuItem("Window/Level Editor")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(LevelEditorWindow));
  }

  void OnGUI() {
    this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
    EditorGUILayout.BeginVertical();

    this.DrawActions();

    GUILayout.Space(SectionSpace);

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.LabelField("Level Dimensions");
    gridSize.Width = EditorGUILayout.IntField(gridSize.Width);
    gridSize.Height = EditorGUILayout.IntField(gridSize.Height);
    this.levelGrid.Reallocate(this.gridSize.Width, this.gridSize.Height);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal(new [] { GUILayout.MaxWidth(300) });
    EditorGUILayout.LabelField("Axis of Symmetry");
    if (GUILayout.Toggle(this.symmetryAxis == SymmetryAxis.AxisX, "X Axis"))
      this.symmetryAxis = SymmetryAxis.AxisX;
    if (GUILayout.Toggle(this.symmetryAxis == SymmetryAxis.AxisY, "Y Axis"))
      this.symmetryAxis = SymmetryAxis.AxisY;
    EditorGUILayout.EndHorizontal();

    GUILayout.Space(SectionSpace);

    EditorGUILayout.BeginHorizontal();
    this.DrawPalette();
    this.DrawGrid();
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.EndVertical();
    EditorGUILayout.EndScrollView();
  }

  private void DrawActions() {
    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Save Level"))
      this.SaveLevel();
    GUILayout.Button("Load Level");
    EditorGUILayout.EndHorizontal();
  }

  private void DrawPalette() {
    EditorGUILayout.BeginVertical();
    EditorGUILayout.LabelField("Palette");
    this.DrawPaletteItem(null);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall0Edge0);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall1Edge0);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall1Edge1);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall1Edge2);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall1Edge3);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall2Edge0);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall2Edge1);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall2Edge2);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall2Edge3);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall2Edge4);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall2Edge5);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall3Edge0);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall3Edge1);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall3Edge2);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall3Edge3);
    this.DrawPaletteItem(LevelModels.GridItemType.Wall4Edge0);
    this.DrawPaletteItem(LevelModels.GridItemType.Player0);
    this.DrawPaletteItem(LevelModels.GridItemType.Player1);
    this.DrawPaletteItem(LevelModels.GridItemType.Goal);
    EditorGUILayout.EndVertical();
  }

  private void DrawPaletteItem(LevelModels.GridItemType? item) {
    if (item.HasValue) {
      if (GUILayout.Button(PaletteItemTexture(item.Value), new [] { GUILayout.Width(100) }))
        this.currentPaletteItem = item.Value;
    } else {
      if (GUILayout.Button(string.Empty, new [] { GUILayout.Width(100) }))
        this.currentPaletteItem = null;
    }
  }

  private void DrawGrid() {
    EditorGUILayout.BeginVertical();
    for (int y = 0; y < this.gridSize.Height; ++y) {
      if (this.symmetryAxis == SymmetryAxis.AxisY && this.gridSize.Height / 2 == y)
        GUILayout.Space(SymmetryLineSpace);

      EditorGUILayout.BeginHorizontal();
      for (int x = 0; x < this.gridSize.Width; ++x) {
        if (this.symmetryAxis == SymmetryAxis.AxisX && this.gridSize.Width / 2 == x)
          GUILayout.Space(SymmetryLineSpace);

        this.DrawGridButton(x, y);
      }
      EditorGUILayout.EndHorizontal();
    }
    EditorGUILayout.EndVertical();
  }

  private void DrawGridButton(int x, int y) {
    var item = this.levelGrid.GetItem(x, y);
    bool clicked = false;
    var options = new [] { GUILayout.Width(30), GUILayout.Height(30) };
    if (item.HasValue) {
      clicked = GUILayout.Button(PaletteItemTexture(item.Value), options);
    } else {
      clicked = GUILayout.Button(string.Empty, options);
    }
    if (clicked)
      this.levelGrid.SetItem(x, y, this.currentPaletteItem);
  }

  private static Texture PaletteItemTexture(LevelModels.GridItemType item) {
    return (Texture) EditorGUIUtility.Load($"Assets/Textures/Editor/{item.ToString()}.png");
  }

  private void SaveLevel() {
    var path = EditorUtility.SaveFilePanel("Save Level", string.Empty, "Level", ".xml.level");

    if (path.Length > 0) {
      using (var fileStream = File.OpenWrite(path)) {
        var levelModel = this.MakeLevelModel();
        var serialiser = new XmlSerializer(typeof(LevelModels.Level));
        serialiser.Serialize(fileStream, levelModel);
      }
    } else {
      EditorUtility.DisplayDialog("No File Selected", "A file must be selected to save", "OK");
    }
  }

  private LevelModels.Level MakeLevelModel() {
    return new LevelModels.Level() {
      Width = this.gridSize.Width,
      Height = this.gridSize.Height,
      Grid = new LevelModels.Grid() {
        GridItems = this.levelGrid.ToFlatGrid()
          .Select(item => {
            return item.HasValue
              ? new LevelModels.GridItem() { Type = item.Value }
              : (LevelModels.AbstractGridItem) new LevelModels.EmptyItem();
          })
          .ToArray()
      }
    };
  }
}
