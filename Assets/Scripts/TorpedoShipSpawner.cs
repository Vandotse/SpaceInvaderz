using UnityEngine;

public class TorpedoShipSpawner : MonoBehaviour
{
    public GameObject[] torpedoShipPrefabs;
    public float baseSpawnInterval = 3f;
    public float spawnZOffset = 0.2f;

    private float timer;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanSpawnTargets)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < baseSpawnInterval) return;
        timer = 0f;

        if (torpedoShipPrefabs == null || torpedoShipPrefabs.Length == 0) return;

        float planeY = transform.position.y;
        float depth = cam.transform.position.y - planeY;

        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));

        float randX = Random.Range(topLeft.x, topRight.x);
        float topZ = topLeft.z;

        Vector3 spawnPos = new Vector3(randX, planeY + 2.0f, topZ + spawnZOffset);

        GameObject prefab = torpedoShipPrefabs[Random.Range(0, torpedoShipPrefabs.Length)];
        GameObject torpedoShip = Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
