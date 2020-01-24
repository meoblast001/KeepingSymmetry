using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow {
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

  private GridSize gridSize = new GridSize() { Width = 10, Height = 10 };
  private SymmetryAxis symmetryAxis = SymmetryAxis.AxisX;
  private PaletteItem currentPaletteItem;

  [MenuItem("Window/Level Editor")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(LevelEditorWindow));
  }

  void OnGUI() {
    EditorGUILayout.BeginVertical();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.LabelField("Level Dimensions");
    gridSize.Width = EditorGUILayout.IntField(gridSize.Width);
    gridSize.Height = EditorGUILayout.IntField(gridSize.Height);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal(new [] { GUILayout.MaxWidth(300) });
    EditorGUILayout.LabelField("Axis of Symmetry");
    if (GUILayout.Toggle(this.symmetryAxis == SymmetryAxis.AxisX, "X Axis"))
      this.symmetryAxis = SymmetryAxis.AxisX;
    if (GUILayout.Toggle(this.symmetryAxis == SymmetryAxis.AxisY, "Y Axis"))
      this.symmetryAxis = SymmetryAxis.AxisY;
    EditorGUILayout.EndHorizontal();

    GUILayout.Space(50);

    EditorGUILayout.BeginHorizontal();
    this.DrawPalette();
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.EndVertical();
  }

  private void DrawPalette() {
    EditorGUILayout.BeginVertical();
    EditorGUILayout.LabelField("Palette");
    this.DrawPaletteItem(PaletteItem.Wall4Edge0);
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

  private void DrawPaletteItem(PaletteItem item) {
    Texture tex = (Texture) EditorGUIUtility.Load($"Assets/Textures/Editor/{item.ToString()}.png");
    if (GUILayout.Button(tex, new [] { GUILayout.Width(100) }))
      this.currentPaletteItem = item;
  }

  private void DrawGrid() {
    // TODO
  }
}
