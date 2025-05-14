using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private bool enableSprinting = true;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Slope Handling")]
    [SerializeField] private bool preventUphillSlowdown = true;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeRayLength = 1.5f;
    
    private Rigidbody rb;
    private bool isSprinting = false;

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

        // Handle sprint toggle
        if (enableSprinting)
        {
            isSprinting = Input.GetKey(sprintKey);
        }
        else
        {
            isSprinting = false;
        }
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        
        if (direction.magnitude < 0.1f)
            return;
            
        // Calculate current speed based on sprinting state
        float currentSpeed = moveSpeed;
        if (isSprinting && enableSprinting)
        {
            currentSpeed = sprintSpeed;
        }

        // Handle slopes if enabled
        if (preventUphillSlowdown)
        {
            direction = AdjustDirectionForSlope(direction);
        }

        rb.AddForce(direction.normalized * currentSpeed, ForceMode.Acceleration);
    }

    private Vector3 AdjustDirectionForSlope(Vector3 direction)
    {
        // Cast ray to detect ground
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            
            // If we're on a slope
            if (slopeAngle > 0 && slopeAngle < maxSlopeAngle)
            {
                // Project our movement direction onto the slope
                Vector3 slopeDirection = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                
                // If we're moving uphill
                if (Vector3.Dot(slopeDirection, Vector3.up) < 0)
                {
                    // Compensate for the slope by applying more force
                    float slopeCompensation = 1.0f + (slopeAngle / maxSlopeAngle);
                    direction = slopeDirection * slopeCompensation;
                }
                else
                {
                    // Just follow the slope for downhill
                    direction = slopeDirection;
                }
            }
        }
        
        return direction;
    }

    // Public methods to toggle features at runtime
    public void SetSprintingEnabled(bool enabled)
    {
        enableSprinting = enabled;
    }

    public void SetUphillCompensationEnabled(bool enabled)
    {
        preventUphillSlowdown = enabled;
    }

    public void SetSprintKey(KeyCode key)
    {
        sprintKey = key;
    }

    // Public method to change sprint multiplier at runtime
    public void SetSprintMultiplier(float multiplier)
    {
        sprintSpeed = Mathf.Max(1.0f, multiplier);
    }

    // Check if player is currently sprinting
    public bool IsSprinting()
    {
        return isSprinting && enableSprinting;
    }
}