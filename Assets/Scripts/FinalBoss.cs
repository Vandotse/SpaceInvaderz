using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float beamDamageInterval = 1f;

    [Header("Torpedo Attack")]
    public GameObject torpedoPrefab;
    public Transform torpedoSpawnPoint;
    public float torpedoSpawnInterval = 2f;

    [Header("Bullet Barrage")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnOrigin;
    public float barrageInterval = 5f;
    public float bulletSpreadWidth = 10f;
    public int bulletsPerWave = 12;
    public int gapWidth = 2;
    public float bulletForwardOffset = 0.5f;

    [Header("Audio")]
    public AudioClip spawnClip;

    private int currentHealth;
    private int effectiveMaxHealth;
    private float torpedoTimer = 0f;
    private float nextBarrageTime = 0f;
    private bool isInBeam = false;
    private float beamDamageAccumulator = 0f;
    private AudioManager audioManager;

    private void Start()
    {
        int loopMultiplier = Mathf.Max(1, PlayerInfo.GameLoopCount);
        effectiveMaxHealth = Mathf.Max(1, maxHealth * loopMultiplier);
        currentHealth = effectiveMaxHealth;
        audioManager = FindObjectOfType<AudioManager>();
        nextBarrageTime = Time.time + barrageInterval;

        if (audioManager != null && spawnClip != null)
        {
            audioManager.Play(spawnClip);
        }
    }

    private void Update()
    {
        HandleBeamDamage();
        HandleTorpedoAttack();
        HandleBulletBarrage();
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
        if (audioManager != null)
        {
            audioManager.Play(audioManager.Explosion);
        }

        GameManager.Instance?.AwardScaledScore(5000);

        PlayerInfo.GameLoopCount = Mathf.Max(1, PlayerInfo.GameLoopCount + 1);
        PlayerInfo.Clear();
        PlayerInfo.ForceFreshStart = true;

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
                GameManager.Instance.ApplyPlayerDamage(GameManager.Instance.GetMaxHealth());
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

    private void HandleTorpedoAttack()
    {
        if (torpedoPrefab == null) return;

        torpedoTimer += Time.deltaTime;
        if (torpedoTimer >= torpedoSpawnInterval)
        {
            torpedoTimer = 0f;
            SpawnTorpedo();
        }
    }

    private void SpawnTorpedo()
    {
        Transform spawnPoint = torpedoSpawnPoint != null ? torpedoSpawnPoint : transform;
        Instantiate(torpedoPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void HandleBulletBarrage()
    {
        if (bulletPrefab == null || bulletsPerWave <= 0) return;

        if (Time.time >= nextBarrageTime)
        {
            FireBulletWall();
            nextBarrageTime = Time.time + barrageInterval;
        }
    }

    private void FireBulletWall()
    {
        Transform origin = bulletSpawnOrigin != null ? bulletSpawnOrigin : transform;
        Vector3 basePos = origin.position + origin.forward * bulletForwardOffset;

        int effectiveGap = Mathf.Clamp(gapWidth, 1, Mathf.Max(1, bulletsPerWave - 1));
        int gapStart = Random.Range(0, Mathf.Max(1, bulletsPerWave - effectiveGap));

        float width = bulletSpreadWidth;
        float step = bulletsPerWave > 1 ? width / (bulletsPerWave - 1) : 0f;
        float startX = basePos.x - width * 0.5f;

        for (int i = 0; i < bulletsPerWave; i++)
        {
            if (i >= gapStart && i < gapStart + effectiveGap)
            {
                continue;
            }

            Vector3 spawnPos = new Vector3(startX + step * i, basePos.y, basePos.z);
            Instantiate(bulletPrefab, spawnPos, bulletPrefab.transform.rotation);
        }
    }
}

