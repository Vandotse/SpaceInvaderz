using UnityEngine;

public class asteroidMove : MonoBehaviour
{
    public float speed = 3f;
    public float destroyY = -10f;

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
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Beam"))
        {
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Player"))
        {
            Time.timeScale = 0f;
        }
    }
}
