using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player Camera Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float headBobbingIntensity = 0.075f;
    [SerializeField] private float headBobbingSpeed = 17.5f;

    [Header("FOV Settings")]
    [SerializeField] private float insanityFovIncrease = 20f;
    [SerializeField] private float sprintFovIncrease = 20f;
    [SerializeField] private float fovLerpSpeed = 5f;

    private bool isSprinting;
    private float xRotation = 0f;

    private Camera cam;
    private Rigidbody rb;

    

    private void Awake()
    {
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();    
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMouseInput();

        
    }

    void HandleMouseInput()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera vertically
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Follow player
        float bobbing = Mathf.Sin(Time.time * headBobbingSpeed) * headBobbingIntensity;
        Vector3 bobbingOffset = new Vector3(0f, bobbing, 0f);
        cam.transform.position = transform.position + cameraOffset + (rb.velocity.magnitude > 0.25f ? bobbingOffset : Vector3.zero);

        
    }

    public void ApplyFov(float currentInsanity)
    {
        const float baseFov = 60f;

        float sprintFov = isSprinting ? sprintFovIncrease : 0f;
        float insanityFov = currentInsanity / 100f * insanityFovIncrease;

        float currentFov = Camera.main.fieldOfView;
        float targetFov = baseFov + sprintFov + insanityFov;
        cam.fieldOfView = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovLerpSpeed);
    }

    public void ToggleSprintFov(bool toggle)
    {
        isSprinting = toggle;
    }

    
}
