using UnityEngine;

public class MiniBoss1 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float directionChangeInterval = 1f;

    [Header("Combat")]
    public int maxHealth = 25;
    public float fireInterval = 0.1f;
    public GameObject bulletPrefab;
    public float beamDamageInterval = 1f;

    [Header("Beam Attack")]
    public GameObject beamPrefab;
    public float beamCooldown = 10f;
    public float beamDuration = 3f;
    public float beamSpawnDistance = 1.0f;

    [Header("Audio")]
    public AudioClip spawnClip;

    private int currentHealth;
    private int effectiveMaxHealth;
    private float nextFireTime = 0f;
    private float nextDirectionChange = 0f;
    private float direction = 1f;
    private bool isInBeam = false;
    private float beamDamageAccumulator = 0f;
    private bool beamActive = false;
    private float nextBeamTime = 0f;
    private float beamEndTime = 0f;
    private GameObject activeBeam;

    private Camera cam;
    private AudioManager audioManager;
    private float leftBoundary;
    private float rightBoundary;

    private void Start()
    {
        cam = Camera.main;
        audioManager = FindObjectOfType<AudioManager>();

        if (cam != null)
        {
            float depth = cam.transform.position.y - transform.position.y;
            Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.9f, depth));
            Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.9f, depth));
            leftBoundary = left.x;
            rightBoundary = right.x;
        }

        direction = Random.Range(0, 2) == 0 ? -1f : 1f;
        nextDirectionChange = Time.time + directionChangeInterval;
        nextFireTime = Time.time + fireInterval * 0.5f;
        int loopMultiplier = Mathf.Max(1, PlayerInfo.GameLoopCount);
        effectiveMaxHealth = Mathf.Max(1, maxHealth * loopMultiplier);
        currentHealth = effectiveMaxHealth;
        nextBeamTime = Time.time + beamCooldown;

        if (audioManager != null && spawnClip != null)
        {
            audioManager.Play(spawnClip);
        }
    }

    private void Update()
    {
        Move();
        HandleBeamDamage();
        HandleBeamAttack();

        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void Move()
    {
        Vector3 pos = transform.position;
        pos.x += direction * moveSpeed * Time.deltaTime;

        if (pos.x <= leftBoundary+1.5f)
        {
            pos.x = leftBoundary+1.5f;
            direction = 1f; 
        }
        else if (pos.x >= rightBoundary-1.5f)
        {
            pos.x = rightBoundary-1.5f;
            direction = -1f;
        }

        if (Time.time >= nextDirectionChange)
        {
            direction = Random.Range(0, 2) == 0 ? -1f : 1f;
            nextDirectionChange = Time.time + directionChangeInterval;
        }

        transform.position = pos;
    }

    private void Fire()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.back * 0.5f;
        Instantiate(bulletPrefab, spawnPos, bulletPrefab.transform.rotation);
    }

    private void ApplyDamage(int amount)
    {
        if (amount <= 0) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        StopBeam(true);

        if (audioManager != null)
        {
            audioManager.Play(audioManager.Explosion);
        }

        GameManager.Instance?.AwardScaledScore(2500);

        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.NotifyMiniBossDefeated();
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Destroy(collision.collider.gameObject);
            ApplyDamage(1);
        }
        else if (collision.collider.CompareTag("Beam"))
        {
            isInBeam = true;
        }
        else if (collision.collider.CompareTag("Player"))
        {
            if (audioManager != null)
            {
                audioManager.Play(audioManager.PlayerHit);
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerHit();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Beam"))
        {
            isInBeam = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Beam"))
        {
            isInBeam = false;
            beamDamageAccumulator = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            isInBeam = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            isInBeam = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            isInBeam = false;
            beamDamageAccumulator = 0f;
        }
    }

    private void HandleBeamDamage()
    {
        if (!isInBeam) return;

        beamDamageAccumulator += Time.deltaTime;
        if (beamDamageAccumulator >= beamDamageInterval)
        {
            int ticks = Mathf.FloorToInt(beamDamageAccumulator / beamDamageInterval);
            beamDamageAccumulator -= ticks * beamDamageInterval;
            ApplyDamage(ticks);
        }
    }

    private void HandleBeamAttack()
    {
        if (beamPrefab == null) return;

        if (!beamActive && Time.time >= nextBeamTime)
        {
            StartBeam();
        }
        else if (beamActive && Time.time >= beamEndTime)
        {
            StopBeam();
        }
    }

    private void StartBeam()
    {
        if (beamPrefab == null) return;

        float spawnDistance = Mathf.Abs(beamSpawnDistance);
        Vector3 beamDirection = -transform.forward;
        Vector3 spawnPos = transform.position + beamDirection * spawnDistance;
        Quaternion rotation = Quaternion.LookRotation(beamDirection) * Quaternion.Euler(90f, 0f, 0f);

        activeBeam = Instantiate(beamPrefab, spawnPos, rotation, transform);
        beamActive = true;
        beamEndTime = Time.time + beamDuration;
    }

    private void StopBeam(bool forceImmediate = false)
    {
        if (!beamActive && !forceImmediate) return;

        if (activeBeam != null)
        {
            Destroy(activeBeam);
            activeBeam = null;
        }

        beamActive = false;
        beamEndTime = 0f;
        beamDamageAccumulator = 0f;
        if (!forceImmediate)
        {
            nextBeamTime = Time.time + beamCooldown;
        }
    }
}
