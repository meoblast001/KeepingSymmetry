using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelPaletteSet", menuName = "ScriptableObjects/LevelPaletteSet")]
public class LevelPaletteSet : ScriptableObject {
  [Serializable]
  public struct LevelPaletteEntry {
    public string id;
    public LevelPalette levelPalette;
  }

  public LevelPaletteEntry[] levelPalettes;

  public LevelPalette? GetById(string id) {
    foreach (var entry in this.levelPalettes) {
      if (entry.id == id)
        return entry.levelPalette;
    }
    return null;
  }
}
