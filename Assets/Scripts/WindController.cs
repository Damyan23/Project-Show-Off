using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class WindController : MonoBehaviour
{
    [Header("Wind Base Settings")]
    public Vector3 baseDirection = Vector3.forward;
    public float baseStrength = 1f;

    [Header("Wind Variation Settings")]
    public float directionVariationSpeed = 0.1f;
    public float strengthNoiseSpeed = 0.5f;
    public float strengthVariationAmplitude = 0.5f;

    [Header("WindZone Target (Optional)")]
    public WindZone targetWindZone;

    [Header("Material/Shader Global Target")]
    public string globalWindDirectionName = "_GlobalWindDirection";
    public string globalWindStrengthName = "_WindStrength";
    [SerializeField] private VisualEffect smokeVFX;

    [Header("Smoke VFX Settings")]
    public float smokeAccelerationMultiplier = 1f;

    private float timeOffset;

    void Start()
    {
        timeOffset = Random.Range(0f, 1000f);
        
        // Reset VFX to prevent offset issues
        if (smokeVFX != null)
        {
            smokeVFX.Stop();
            smokeVFX.Play();
        }
    }

    void Update()
    {
        // Don't apply forces during the first few frames to let VFX initialize
        if (Time.time < 0.1f) return;

        float time = Time.time + timeOffset;

        // Compute animated strength with Perlin noise
        float noiseValue = Mathf.PerlinNoise(time * strengthNoiseSpeed, 0f);
        float dynamicStrength = baseStrength * (1 + (noiseValue - 0.5f) * 2 * strengthVariationAmplitude);

        // Rotate base direction slightly over time
        Quaternion rotation = Quaternion.Euler(0f, Mathf.Sin(time * directionVariationSpeed) * 45f, 0f);
        Vector3 dynamicDirection3D = rotation * baseDirection.normalized;

        // For shaders: Convert to Vector2 (XZ plane) for horizontal wind direction
        Vector2 windDirection2D = new Vector2(dynamicDirection3D.x, dynamicDirection3D.z).normalized;

        // Set shader globals
        Shader.SetGlobalVector(globalWindDirectionName, windDirection2D);
        Shader.SetGlobalFloat(globalWindStrengthName, dynamicStrength);

        // For smoke VFX: Create acceleration vector (direction * strength)
        // Make sure this is in the correct space (usually world space)
        Vector3 smokeAcceleration = new Vector3(
            windDirection2D.x * dynamicStrength * smokeAccelerationMultiplier,
            dynamicStrength, // Usually no vertical wind acceleration, but you can modify this
            windDirection2D.y * dynamicStrength * smokeAccelerationMultiplier
        );

        // Convert to local space if VFX is using local space
        if (smokeVFX.transform.parent != null)
        {
            smokeAcceleration = smokeVFX.transform.InverseTransformDirection(smokeAcceleration);
        }

        smokeVFX.SetVector3("Velocity", smokeAcceleration);

        // Optional: Also update WindZone if assigned
        if (targetWindZone != null)
        {
            targetWindZone.windMain = dynamicStrength;
            targetWindZone.transform.rotation = Quaternion.LookRotation(dynamicDirection3D);
        }

        //Debug.Log($"Wind Direction 2D: {windDirection2D}, Strength: {dynamicStrength}, Smoke Acceleration: {smokeAcceleration}");
        
    }
}