using System.Collections.Generic;
using UnityEngine;

public class TerrainStreamer : MonoBehaviour
{
  public GameObject tilePrefab;
  public Transform origin;
  public float tileLength = 10f;

  public Vector3 axis = new Vector3(0, 0, 1);
  public float speed = 2f;

  public int tilesAhead = 8;
  public int tilesBehind = 2;

  public Vector3 positionOffset = new Vector3(0, -2f, 0);

  public int poolSize = 10;

  public float ScrollDistance { get; private set; }
  public float Speed => speed;

  private Vector3 axisN;
  private readonly Dictionary<int, GameObject> live = new();
  private readonly Queue<GameObject> pool = new();
  private readonly List<int> toRemove = new();

  void Awake() {
    if (!origin) origin = transform;
    axisN = axis.sqrMagnitude > 0f ? axis.normalized : Vector3.forward;
    for (int i = 0; i < poolSize; i++) {
      var go = Instantiate(tilePrefab);
      go.SetActive(false);
      pool.Enqueue(go);
    }
  }

  void Update() {
    ScrollDistance += speed * Time.deltaTime;

    int centerIndex = Mathf.FloorToInt(ScrollDistance / tileLength);
    int firstIndex = centerIndex - tilesBehind;
    int lastIndex  = centerIndex + tilesAhead;

    for (int i = firstIndex; i <= lastIndex; i++)
      if (!live.ContainsKey(i)) live[i] = Spawn(i);

    toRemove.Clear();
    foreach (var kv in live) {
      if (kv.Key < firstIndex || kv.Key > lastIndex) toRemove.Add(kv.Key);
    }
    foreach (int idx in toRemove) Despawn(idx);

    foreach (var kv in live) {
      int idx = kv.Key; var go = kv.Value;
      Vector3 basePos = origin.position + positionOffset;
      Vector3 along   = axisN * ((idx * tileLength) - ScrollDistance);
      go.transform.SetPositionAndRotation(basePos + along, Quaternion.identity);
    }
  }

  [SerializeField, Range(0f, 0.01f)] float edgeBleed = 0.002f;

  GameObject Spawn(int idx) {
  var go = pool.Count > 0 ? pool.Dequeue() : Instantiate(tilePrefab);
  go.transform.SetParent(transform, true);
  go.SetActive(true);

  float s = (tileLength / 10f) * (1f + edgeBleed);
  go.transform.localScale = new Vector3(s, 1f, s);

  return go;
}

  void Despawn(int idx) {
    var go = live[idx];
    live.Remove(idx);
    if (pool.Count < poolSize) { go.SetActive(false); pool.Enqueue(go); }
    else Destroy(go);
  }

  void OnDrawGizmosSelected() {
    var p = origin ? origin.position : transform.position;
    Gizmos.color = Color.green;
    Gizmos.DrawLine(p - axis.normalized * tileLength * 2f, p + axis.normalized * tileLength * 2f);
  }
}
