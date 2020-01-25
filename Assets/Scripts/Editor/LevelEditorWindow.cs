using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow {
  const int StartWidth = 10;
  const int StartHeight= 10;
  const int SectionSpace = 50;
  const int SymmetryLineSpace = 15;

  public enum PaletteItem {
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
  private PaletteItem? currentPaletteItem;
  private LevelEditorGrid<PaletteItem> levelGrid = new LevelEditorGrid<PaletteItem>(StartWidth, StartHeight);

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
    GUILayout.Button("Save Level");
    GUILayout.Button("Load Level");
    EditorGUILayout.EndHorizontal();
  }

  private void DrawPalette() {
    EditorGUILayout.BeginVertical();
    EditorGUILayout.LabelField("Palette");
    this.DrawPaletteItem(null);
    this.DrawPaletteItem(PaletteItem.Wall0Edge0);
    this.DrawPaletteItem(PaletteItem.Wall1Edge0);
    this.DrawPaletteItem(PaletteItem.Wall1Edge1);
    this.DrawPaletteItem(PaletteItem.Wall1Edge2);
    this.DrawPaletteItem(PaletteItem.Wall1Edge3);
    this.DrawPaletteItem(PaletteItem.Wall2Edge0);
    this.DrawPaletteItem(PaletteItem.Wall2Edge1);
    this.DrawPaletteItem(PaletteItem.Wall2Edge2);
    this.DrawPaletteItem(PaletteItem.Wall2Edge3);
    this.DrawPaletteItem(PaletteItem.Wall2Edge4);
    this.DrawPaletteItem(PaletteItem.Wall2Edge5);
    this.DrawPaletteItem(PaletteItem.Wall3Edge0);
    this.DrawPaletteItem(PaletteItem.Wall3Edge1);
    this.DrawPaletteItem(PaletteItem.Wall3Edge2);
    this.DrawPaletteItem(PaletteItem.Wall3Edge3);
    this.DrawPaletteItem(PaletteItem.Wall4Edge0);
    this.DrawPaletteItem(PaletteItem.Player0);
    this.DrawPaletteItem(PaletteItem.Player1);
    this.DrawPaletteItem(PaletteItem.Goal);
    EditorGUILayout.EndVertical();
  }

  private void DrawPaletteItem(PaletteItem? item) {
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

  private static Texture PaletteItemTexture(PaletteItem item) {
    return (Texture) EditorGUIUtility.Load($"Assets/Textures/Editor/{item.ToString()}.png");
  }
}
