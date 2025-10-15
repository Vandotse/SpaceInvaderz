using UnityEngine;

public class rotate : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * 30f);
    }
}
