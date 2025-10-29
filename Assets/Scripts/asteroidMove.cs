using UnityEngine;

public class asteroidMove : MonoBehaviour
{
    public float speed = 3f;
    public float destroyY = -10f;

    private AudioManager audioManager;
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;

        if (transform.position.z < destroyY)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            audioManager.Play(audioManager.Explosion);
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Beam"))
        {
            audioManager.Play(audioManager.Explosion);
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Player"))
        {
            audioManager.Play(audioManager.PlayerHit);
            Time.timeScale = 0f;
        }
    }
}
