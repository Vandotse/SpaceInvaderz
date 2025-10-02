using UnityEngine;

[ExecuteAlways]
public class EarthAnchorAndRotate : MonoBehaviour {
  [Header("Screen Placement")]
  [Range(-0.3f, 1.3f)] public float viewportX = 0.5f;  // allow overscan, if needed
  [Range(-0.3f, 1.3f)] public float viewportY = -0.23f; // <— start slightly below bottom
  public float groundY = 0f;

  [Header("Sync with Streamer")]
  public TerrainStreamer streamer;
  public float degreesPerUnit = 2f;
  public bool rotateContinuously = true;

  float lastScroll;

  void Start() {
    lastScroll = streamer ? streamer.ScrollDistance : 0f;
    Reposition();
  }

  void LateUpdate() {
    Reposition();
    if (rotateContinuously && streamer) {
      float delta = streamer.ScrollDistance - lastScroll;
      transform.Rotate(Vector3.right, -delta * degreesPerUnit, Space.World);
      lastScroll = streamer.ScrollDistance;
    }
  }

  void Reposition() {
    var cam = Camera.main;
    if (!cam) return;

    Ray ray = cam.ViewportPointToRay(new Vector3(viewportX, viewportY, 0f));
    Plane ground = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
    if (ground.Raycast(ray, out float enter)) {
      var hit = ray.GetPoint(enter);
      transform.position = new Vector3(hit.x, groundY, hit.z);
    }
  }
}
