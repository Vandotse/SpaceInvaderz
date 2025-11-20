using UnityEngine;

public class AsteroidBoss : MonoBehaviour
{
    public float fallSpeed = 1.5f;
    public float destroyY = -15f;
    public int maxHealth = 25;
    public float beamDamageInterval = 1f;

    private int currentHealth;
    private AudioManager audioManager;
    private bool isInBeam = false;
    private float beamDamageAccumulator = 0f;

    private void Start()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        transform.position += Vector3.back * fallSpeed * Time.deltaTime;
        HandleBeamDamage();

        if (transform.position.z < destroyY)
        {
            Destroy(gameObject);
        }
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

        GameManager.Instance?.AwardScaledScore(1000);

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
}

