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
    public int width;
    public int height;
  }

  private GridSize gridSize;
  private PaletteItem currentPaletteItem;

  [MenuItem("Window/Level Editor")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(LevelEditorWindow));
  }

  void OnGUI() {
    EditorGUILayout.BeginVertical();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.LabelField("Level Dimensions");
    gridSize.width = EditorGUILayout.IntField(gridSize.width);
    gridSize.height = EditorGUILayout.IntField(gridSize.height);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    this.DrawPalette();
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.EndVertical();
  }

  private void DrawGrid() {}

  private void DrawPalette() {
    EditorGUILayout.BeginVertical(new [] { GUILayout.Width(100) });
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
    EditorGUILayout.EndVertical();
  }

  private void DrawPaletteItem(PaletteItem item) {
    Texture tex = (Texture) EditorGUIUtility.Load($"Assets/Textures/Editor/{item.ToString()}.png");
    GUILayout.Button(tex);
  }
}
