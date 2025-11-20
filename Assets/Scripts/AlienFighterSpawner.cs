using UnityEngine;

public class AlienFighterSpawner : MonoBehaviour
{
    public GameObject[] fighterShipPrefabs;
    public float spawnInterval = 5f;
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
        if (timer < spawnInterval) return;
        timer = 0f;

        if (fighterShipPrefabs == null || fighterShipPrefabs.Length == 0) return;

        float planeY = transform.position.y;
        float depth = cam.transform.position.y - planeY;

        float spawnViewportY = Random.Range(0.85f, 0.95f);
        Vector3 leftPos = cam.ViewportToWorldPoint(new Vector3(0f, spawnViewportY, depth));
        Vector3 rightPos = cam.ViewportToWorldPoint(new Vector3(1f, spawnViewportY, depth));

        float randX = Random.Range(leftPos.x, rightPos.x);
        float spawnZ = leftPos.z;

        Vector3 spawnPos = new Vector3(randX, planeY + 2.0f, spawnZ + spawnZOffset);

        GameObject prefab = fighterShipPrefabs[Random.Range(0, fighterShipPrefabs.Length)];
        GameObject fighter = Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
