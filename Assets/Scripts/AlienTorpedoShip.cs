using UnityEngine;

public class AlienTorpedoShip : MonoBehaviour
{
    public float speed = 3f;
    public float destroyY = -10f;
    public float turnSpeed = 5f;

    private AudioManager audioManager;
    private Transform player;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        Vector3 direction = Vector3.back;

        if (player != null)
        {
            direction = player.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
            }
            else
            {
                direction = Vector3.back;
            }
        }

        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        if (transform.position.z < destroyY)
        {
            Destroy(gameObject);
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
            GameManager.Instance?.AwardScaledScore(50);
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Beam"))
        {
            if (audioManager != null)
            {
                audioManager.Play(audioManager.Explosion);
            }
            GameManager.Instance?.AwardScaledScore(50);
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
