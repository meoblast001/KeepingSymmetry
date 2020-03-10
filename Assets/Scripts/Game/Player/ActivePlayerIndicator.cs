using UnityEngine;

public class ActivePlayerIndicator : MonoBehaviour {
  [SerializeField] private GameObject indicator;

  public void SetActive(bool active) {
    this.indicator.SetActive(active);
  }
}
