using UnityEngine;

public class bulletMove : MonoBehaviour
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
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);
        }
    }
}
