using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private KeyCode sprintKey;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float sprintFovIncrease = 20f;
    private Rigidbody rb;

    [Header("Sanity")]
    [SerializeField] float enemySightInsanityRange = 50f;
    [SerializeField] float sightInsanityFactor = 2f;
    [SerializeField] float hitInsanityFactor = 25f;
    [SerializeField] float enemyDisableTimeAfterHit = 10f;
    [SerializeField] float insanityFovIncrease = 20f;


    [Header("Camera")]
    [SerializeField] private float fovLerpSpeed = 5f;
    [SerializeField, Range(0f, 1f)] private float maxVignetteIntensity = 0.5f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector3 cameraOffset;
    private Transform cameraTransform;

    [Header("References")]
    [SerializeField] FogController fogController;
    [SerializeField] Volume globalVolume;

    private float xRotation = 0f;
    private float horizontal;
    private float vertical;

    private float currentInsanity;
    private const float maxSanity = 100f;

    const float baseFov = 60f;

    bool isSprinting;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        InvokeRepeating("DetectEnemies", 0f, 1f);
    }

    void Update()
    {
        HandleMouseLook();
        HandleInput();
        ApplyFov();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    //Detect whether enemy is in sight and make player go insane (doesn't stack)
    void DetectEnemies()
    {
        if (Camera.main == null) return;

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) > enemySightInsanityRange) continue;

            if (ObjectVisible(enemy.gameObject))
            {
                AddInsanity(sightInsanityFactor);
                ApplyInsanity();
                return;
            }
        }
    }

    public IEnumerator HitPlayer(EnemyController enemy)
    {
        AddInsanity(hitInsanityFactor);
        ApplyInsanity();

        enemy.gameObject.SetActive(false);

        yield return new WaitForSeconds(enemyDisableTimeAfterHit);

        enemy.gameObject.SetActive(true);
        enemy.transform.position = enemy.points[0];
    }

    public void AddInsanity(float value)
    {
        currentInsanity = Mathf.Clamp(currentInsanity + value, 0, maxSanity);
    }

    void ApplyInsanity()
    {
        fogController.SetFogPercentage(currentInsanity);

        if(globalVolume.profile.TryGet<Vignette>(out Vignette vignette))
        {
            float vignetteIntensity = currentInsanity / 100f * maxVignetteIntensity;
            vignette.intensity.value = vignetteIntensity;
        }
    }

    void ApplyFov()
    {
        float sprintFov = isSprinting ? sprintFovIncrease : 0f;
        float insanityFov = currentInsanity / 100f * insanityFovIncrease;

        float currentFov = Camera.main.fieldOfView;
        float targetFov = baseFov + sprintFov + insanityFov;
        Camera.main.fieldOfView = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovLerpSpeed);
    }

    bool ObjectVisible(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds)) return false;

        if (Physics.Raycast(transform.position, Vector3.Normalize(obj.transform.position - transform.position), out RaycastHit hitInfo))
        {
            return hitInfo.transform.name == obj.name;
        }

        return false;
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
        isSprinting = Input.GetKey(sprintKey);
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        float speed = isSprinting ? sprintSpeed : moveSpeed;
        rb.AddForce(direction.normalized * speed);
    }
}
