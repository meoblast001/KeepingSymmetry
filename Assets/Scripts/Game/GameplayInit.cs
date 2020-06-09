using UnityEngine;
using Zenject;

public class GameplayInit : MonoBehaviour {
  [SerializeField] private string level;

  [Inject] private LevelManager levelManager;

  void Start() {
    this.levelManager.Load(this.level);
  }
}
