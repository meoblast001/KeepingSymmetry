using Zenject;

public class GameplayInstaller : MonoInstaller {
  public override void InstallBindings() {
    Container.BindInterfacesAndSelfTo<SceneGrid>().AsSingle();
  }
}
