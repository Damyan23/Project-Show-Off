using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(SanityController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private bool enableSprinting = true;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    private bool isSprinting = false;
    
    private float horizontal;
    private float vertical;

    [Header("Slope Handling")]
    [SerializeField] private bool preventUphillSlowdown = true;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeRayLength = 1.5f;
    

    public Rigidbody rb { get; private set; }

    [Header("References")]
    [SerializeField] private FogController fogController;
    [SerializeField] private CameraController cameraController;

    private bool isActive = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isActive) return;

        HandleInput();

        cameraController.ToggleSprintFov(isSprinting);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }    

    public void TogglePlayerMovement(bool toggle)
    {
        isActive = toggle;
    }

    void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(sprintKey);
    }

    private void MovePlayer()
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