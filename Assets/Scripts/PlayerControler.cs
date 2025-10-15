using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public InputActionReference Primary;
    public InputActionReference Move;
    public float moveSpeed = 0.03f;
    public GameObject BulletPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Move.action.ReadValue<Vector2>();
        transform.position += new Vector3(move.x * moveSpeed, 0, move.y * moveSpeed);
    }

    private void OnEnable()
    {
        Primary.action.started += OnPrimary;
    }

    private void OnDisable()
    {
        Primary.action.started -= OnPrimary;
    }

    private void OnPrimary(InputAction.CallbackContext context)
    {
        Instantiate(BulletPrefab, transform.position, Quaternion.identity);
    }
}
