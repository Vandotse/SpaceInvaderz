using UnityEngine;

public class asteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs;
    public float baseSpawnInterval = 2f;
    public float fallSpeed = 3f;
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

        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        float planeY = transform.position.y;
        float depth = cam.transform.position.y - planeY;

        Vector3 topLeft  = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));

        float randX = Random.Range(topLeft.x, topRight.x);
        float topZ  = topLeft.z;

        Vector3 spawnPos = new Vector3(randX, planeY + 2.0f, topZ + spawnZOffset);

        GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        GameObject asteroid = Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
