using UnityEngine;

public class MiniBossSpawner : MonoBehaviour
{
    public GameObject miniBossPrefab;
    public float spawnDelay = 5f;
    public Transform spawnPoint;
    public float viewportMinY = 0.7f;
    public float viewportMaxY = 0.85f;

    private bool hasSpawned = false;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.BeginMiniBossEncounter();
        }

        Invoke(nameof(SpawnMiniBoss), spawnDelay);
    }

    private void SpawnMiniBoss()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanSpawnTargets)
        {
            Invoke(nameof(SpawnMiniBoss), 0.25f);
            return;
        }

        if (hasSpawned) return;
        if (miniBossPrefab == null) return;

        Vector3 spawnPos;
        Quaternion spawnRot = transform.rotation;

        if (spawnPoint != null)
        {
            spawnPos = spawnPoint.position;
            spawnRot = spawnPoint.rotation;
        }
        else if (cam != null)
        {
            float planeY = transform.position.y;
            float depth = cam.transform.position.y - planeY;
            float viewportY = Random.Range(viewportMinY, viewportMaxY);

            Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, viewportY, depth));
            Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, viewportY, depth));

            float randX = Random.Range(left.x, right.x);
            float spawnZ = left.z;

            spawnPos = new Vector3(randX, planeY + 2.0f, spawnZ);
        }
        else
        {
            spawnPos = transform.position;
        }

        Instantiate(miniBossPrefab, spawnPos, spawnRot);
        hasSpawned = true;
    }
}

