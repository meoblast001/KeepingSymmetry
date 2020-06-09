using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller {
  [SerializeField] private GameObject playerMovementControllerPrefab;
  [SerializeField] private GameObject player1Prefab;
  [SerializeField] private GameObject player2Prefab;
  [SerializeField] private LevelPaletteSet levelPaletteSet;

  public override void InstallBindings() {
    Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle();
    Container.BindInterfacesAndSelfTo<SceneGrid>().AsSingle();
    Container.BindInterfacesAndSelfTo<PlayerMovementController>()
      .FromComponentInNewPrefab(this.playerMovementControllerPrefab)
      .AsSingle();
    Container.Bind<SceneGridActor>().WithId(PlayerType.Player1)
      .FromComponentInNewPrefab(this.player1Prefab)
      .AsCached();
    Container.Bind<SceneGridActor>().WithId(PlayerType.Player2)
      .FromComponentInNewPrefab(this.player2Prefab)
      .AsCached();
    Container.Bind<LevelPaletteSet>().FromInstance(this.levelPaletteSet).AsSingle();
  }
}
