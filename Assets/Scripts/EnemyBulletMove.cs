using UnityEngine;

public class EnemyBulletMove : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 30f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerHit();
            }
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Beam"))
        {
            Destroy(gameObject);
        }
    }
    

}
