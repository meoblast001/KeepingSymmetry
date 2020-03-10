using System;
using UnityEngine;

[Serializable]
public struct Point {
  public int x;
  public int y;

  public Vector3 ToVector3OnXZPlane() {
    return new Vector3(x, 0f, y);
  }
}
