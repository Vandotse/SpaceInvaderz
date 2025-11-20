using UnityEngine;

public class AlienFighterShip : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fireInterval = 2f;
    public GameObject bulletPrefab;
    public float destroyY = -10f;

    private Camera cam;
    private float direction = 1f;
    private float nextDirectionChange = 0f;
    private float directionChangeInterval = 2f;
    private float nextFireTime = 0f;
    private float leftBoundary;
    private float rightBoundary;
    private AudioManager audioManager;

    void Start()
    {
        cam = Camera.main;
        audioManager = FindObjectOfType<AudioManager>();
        
        float depth = cam.transform.position.y - transform.position.y;
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
        
        leftBoundary = topLeft.x;
        rightBoundary = topRight.x;
        
        direction = Random.Range(0, 2) == 0 ? -1f : 1f;
        nextDirectionChange = Time.time + Random.Range(1f, 3f);
        nextFireTime = Time.time + fireInterval;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x += direction * moveSpeed * Time.deltaTime;
        
        if (pos.x <= leftBoundary)
        {
            pos.x = leftBoundary;
            direction = 1f;
        }
        else if (pos.x >= rightBoundary)
        {
            pos.x = rightBoundary;
            direction = -1f;
        }
        
        if (Time.time >= nextDirectionChange)
        {
            direction = Random.Range(0, 2) == 0 ? -1f : 1f;
            nextDirectionChange = Time.time + Random.Range(1f, 3f);
        }
        
        transform.position = pos;
        
        if (Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireInterval;
        }
        
        if (transform.position.z < destroyY)
        {
            Destroy(gameObject);
        }
    }

    void FireBullet()
    {
        if (bulletPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.back * 1f;
            Instantiate(bulletPrefab, spawnPos, bulletPrefab.transform.rotation);
            
            if (audioManager != null)
            {
                audioManager.Play(audioManager.Attack);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            if (audioManager != null)
            {
                audioManager.Play(audioManager.Explosion);
            }
            Destroy(collision.collider.gameObject);
            GameManager.Instance?.AwardScaledScore(100);
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Beam"))
        {
            if (audioManager != null)
            {
                audioManager.Play(audioManager.Explosion);
            }
            GameManager.Instance?.AwardScaledScore(100);
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Player"))
        {
            if (audioManager != null)
            {
                audioManager.Play(audioManager.PlayerHit);
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerHit();
            }
            Destroy(gameObject);
        }
    }
}
