using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    private Rigidbody rb;

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector3 cameraOffset;
    private Transform cameraTransform;

    private float xRotation = 0f;
    private float horizontal;
    private float vertical;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMouseLook();
        HandleInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera vertically
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Follow player
        cameraTransform.position = transform.position + cameraOffset;
    }

    void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        rb.AddForce(direction.normalized * moveSpeed);
    }
}
