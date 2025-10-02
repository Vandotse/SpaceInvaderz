using UnityEngine;

[ExecuteAlways]
public class AnchorToViewport : MonoBehaviour {
  [Range(0f,1f)] public float viewportX = 0.5f;
  [Range(0f,1f)] public float viewportY = 0.2f;
  [Tooltip("World Y of your ground (usually 0).")]
  public float groundY = 0f;

  void LateUpdate() {
    var cam = Camera.main;
    if (!cam) return;

    float dist = cam.transform.position.y - groundY;
    var world = cam.ViewportToWorldPoint(new Vector3(viewportX, viewportY, dist));

    transform.position = new Vector3(world.x, groundY, world.z);
  }
}
