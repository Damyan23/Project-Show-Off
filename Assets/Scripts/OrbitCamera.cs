using UnityEngine;

[ExecuteAlways]
public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Orbit Settings")]
    [Tooltip("Distance from the target.")]
    public float radius = 5f;

    [Tooltip("Horizontal orbit angle (Y axis).")]
    [Range(0f, 360f)]
    public float horizontalAngle = 0f;

    [Tooltip("Vertical tilt of the camera (X rotation).")]
    [Range(-89f, 89f)]
    public float verticalAngle = 30f;

    [Tooltip("Height offset from the target position.")]
    public float heightOffset = 0f;

    [Header("Auto Rotation")]
    [Tooltip("Degrees per second to rotate horizontally.")]
    public float rotationSpeed = 30f;

    [Header("Look Settings")]
    public bool lookAtTarget = true;

    private void Update()
    {
        if (target == null) return;

        // Only apply deltaTime when in Play mode
        float deltaAngle = Application.isPlaying ? rotationSpeed * Time.deltaTime : 0f;

        // Update horizontal angle
        horizontalAngle += deltaAngle;
        horizontalAngle %= 360f; // Keep within 0-360

        // Convert angles to radians
        float hRad = horizontalAngle * Mathf.Deg2Rad;
        float vRad = verticalAngle * Mathf.Deg2Rad;

        // Calculate horizontal position offset
        float horizontalDistance = Mathf.Cos(vRad) * radius;
        float yOffset = Mathf.Sin(vRad) * radius;

        Vector3 offset = new Vector3(
            Mathf.Cos(hRad) * horizontalDistance,
            yOffset + heightOffset,
            Mathf.Sin(hRad) * horizontalDistance
        );

        // Set camera position
        transform.position = target.position + offset;

        // Look at target or use fixed tilt
        if (lookAtTarget)
        {
            transform.LookAt(target.position + Vector3.up * heightOffset);
        }
        else
        {
            transform.rotation = Quaternion.Euler(verticalAngle, -horizontalAngle, 0f);
        }
    }
}
