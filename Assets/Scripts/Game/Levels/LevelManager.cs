using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class LevelManager : MonoBehaviour {
  [SerializeField] private string firstLevel;
  [SerializeField] private LevelPalette defaultLevelPalette;
  [SerializeField] private PlayerMovementController playerMovementController;

  private List<GameObject> activeObjects = new List<GameObject>();

  void Start() {
    this.Load(this.firstLevel);
  }

  private void Load(string levelResourceFile) {
    foreach (var activeObject in this.activeObjects) {
      GameObject.Destroy(activeObject);
    }
    this.activeObjects = new List<GameObject>();

    var levelModel = this.LoadLevelModel(levelResourceFile);
    if (levelModel == null)
      return;

    this.playerMovementController.SymmetryAxis = levelModel.SymmetryAxis;
    for (int y = 0; y < levelModel.Height; ++y) {
      for (int x = 0; x < levelModel.Width; ++x) {
        if (levelModel.Grid.GridItems.Length <= y * levelModel.Width + x) {
          Debug.LogError($"No grid item available at ({x}, {y}) although width/height allow for it");
          break;
        }
        var gridItem = levelModel.Grid.GridItems[y * levelModel.Width + x] as LevelModels.GridItem;
        if (gridItem != null) {
          var objectPrefab = this.SelectPrefab(gridItem.Type);
          var gameObject = GameObject.Instantiate(objectPrefab);
          var sceneGridActor = gameObject.GetComponent<SceneGridActor>();
          if (sceneGridActor != null) {
            sceneGridActor.SetGridPoint(new Point() { x = x, y = y });
            this.activeObjects.Add(gameObject);
          } else {
            Debug.LogError($"Game object instantiated for grid type {gridItem.Type.ToString()} does not contain "
              + "SceneGridActory component");
            GameObject.Destroy(gameObject);
          }
        }
      }
    }
  }

  private LevelModels.Level LoadLevelModel(string levelResourceFile) {
    try {
      var levelResourceText = Resources.Load<TextAsset>(levelResourceFile);
      var serializer = new XmlSerializer(typeof(LevelModels.Level));
      using (var reader = new StringReader(levelResourceText.text)) {
        return (LevelModels.Level) serializer.Deserialize(reader);
      }
    } catch (InvalidOperationException e) {
      Debug.LogError($"Deserialisation error while opening level: {e.Message}");
      return null;
    }
  }

  private GameObject SelectPrefab(LevelModels.GridItemType type) {
    switch (type) {
      case LevelModels.GridItemType.Wall0Edge0:
        return this.defaultLevelPalette.wall0Edge0;
      case LevelModels.GridItemType.Wall1Edge0:
        return this.defaultLevelPalette.wall1Edge0;
      case LevelModels.GridItemType.Wall1Edge1:
        return this.defaultLevelPalette.wall1Edge1;
      case LevelModels.GridItemType.Wall1Edge2:
        return this.defaultLevelPalette.wall1Edge2;
      case LevelModels.GridItemType.Wall1Edge3:
        return this.defaultLevelPalette.wall1Edge3;
      case LevelModels.GridItemType.Wall2Edge0:
        return this.defaultLevelPalette.wall2Edge0;
      case LevelModels.GridItemType.Wall2Edge1:
        return this.defaultLevelPalette.wall2Edge1;
      case LevelModels.GridItemType.Wall2Edge2:
        return this.defaultLevelPalette.wall2Edge2;
      case LevelModels.GridItemType.Wall2Edge3:
        return this.defaultLevelPalette.wall2Edge3;
      case LevelModels.GridItemType.Wall2Edge4:
        return this.defaultLevelPalette.wall2Edge4;
      case LevelModels.GridItemType.Wall2Edge5:
        return this.defaultLevelPalette.wall2Edge5;
      case LevelModels.GridItemType.Wall3Edge0:
        return this.defaultLevelPalette.wall3Edge0;
      case LevelModels.GridItemType.Wall3Edge1:
        return this.defaultLevelPalette.wall3Edge1;
      case LevelModels.GridItemType.Wall3Edge2:
        return this.defaultLevelPalette.wall3Edge2;
      case LevelModels.GridItemType.Wall3Edge3:
        return this.defaultLevelPalette.wall3Edge3;
      case LevelModels.GridItemType.Wall4Edge0:
        return this.defaultLevelPalette.wall4Edge0;
      case LevelModels.GridItemType.Player0:
        // Nothing to instantiate. Special logic for this case.
        return null;
      case LevelModels.GridItemType.Player1:
        // Nothing to instantiate. Special logic for this case.
        return null;
      case LevelModels.GridItemType.Goal:
        return this.defaultLevelPalette.goal;
      default:
        Debug.LogError($"Cannot select prefab for grid item type {type.ToString()}");
        return null;
    }
  }
}
