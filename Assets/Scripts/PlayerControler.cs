using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public InputActionReference Primary;
    public InputActionReference Secondary;
    public InputActionReference Move;
    public float moveSpeed = 0.03f;
    public GameObject BulletPrefab;
    public GameObject BeamPrefab;
    public float primaryDelay = 0.25f;
    
    private bool primaryDown = false;
    private float primaryNextTime = 0f;
    private bool beamActive = false;
    private bool beamCooldown = false;
    private float beamTimer = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        primaryDown = false;
        primaryNextTime = 0f;
    }

    void Update()
    {
        Vector2 move = Move.action.ReadValue<Vector2>();
        transform.position += new Vector3(move.x * moveSpeed, 0, move.y * moveSpeed);

        Vector3 pos = cam.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = cam.ViewportToWorldPoint(pos);

        if (primaryDown == true && Time.time >= primaryNextTime) {
            float spawnDistance = 1f;

            Vector3 spawnPos = transform.position + transform.forward * spawnDistance;

            Instantiate(BulletPrefab, spawnPos, BulletPrefab.transform.rotation);

            primaryNextTime = Time.time + primaryDelay;
        }

        if (beamActive)
        {
            beamTimer += Time.deltaTime;
            if (beamTimer >= 2f)
            {
                StopBeam();
                beamCooldown = true;
                beamTimer = 0f;
            }
        }
        else if (beamCooldown)
        {
            beamTimer += Time.deltaTime;
            if (beamTimer >= 10f)
            {
                beamCooldown = false;
                beamTimer = 0f;
            }
        }
    }

    private void OnEnable()
    {
        Primary.action.performed += OnPrimaryDown;
        Primary.action.canceled += OnPrimaryUp;
        Secondary.action.started += OnBeamStart;
        Secondary.action.canceled += OnBeamEnd;
    }

    private void OnDisable()
    {
        Primary.action.performed -= OnPrimaryDown;
        Primary.action.canceled -= OnPrimaryUp;
        Secondary.action.started -= OnBeamStart;
        Secondary.action.canceled -= OnBeamEnd;
    }

    private void OnPrimaryDown(InputAction.CallbackContext context)
    {
        primaryDown = true;
    }

    private void OnPrimaryUp(InputAction.CallbackContext context)
    {
        primaryDown = false;
    }

    private void OnBeamStart(InputAction.CallbackContext context)
    {
        float spawnDistance = 1f;

        Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
        if (beamCooldown || beamActive) return;
        beamActive = true;
        Quaternion BeamRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
        Instantiate(BeamPrefab, spawnPos, BeamRotation, transform);
    }

    private void OnBeamEnd(InputAction.CallbackContext context)
    {
        if (beamActive)
            StopBeam();
    }

    private void StopBeam()
    {
        foreach (Transform child in transform)
            if (child.CompareTag("Beam"))
                Destroy(child.gameObject);

        beamActive = false;
    }
}
