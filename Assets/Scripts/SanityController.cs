using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SanityController : MonoBehaviour
{
    public float CurrentInsanity { get; private set; }
    const float maxSanity = 100f;

    [Header("Insanity Settings")]
    [SerializeField] private float hitInsanityPoints;
    [SerializeField] private float enemyDetectionRange = 50f;
    [SerializeField] private float enemyDetectionInsanityPoints = 4f;
    [SerializeField] float offerSanityReduction = 20f;

    [Header("Post-Processing Settings")]
    [SerializeField] private float maxChromaticAberration = 0.5f;
    [SerializeField] private float maxMotionBlur = 1.0f;
    [SerializeField] private float maxDesaturation = -100f;
    [SerializeField, Range(0f, 1f)] private float maxVignetteIntensity = 0.5f;

    [Header("Layer Masking")]
    [SerializeField] private LayerMask exclusionLayer;

    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private FogController fogController;
    [SerializeField] private CameraController cameraController;

    [Header("Debug")]
    [SerializeField] private bool enableDebugMode = false;

    #region Volume Overrides

    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private MotionBlur motionBlur;
    private Vignette vignette;

    #endregion

    private void Start()
    {
        InventoryManager.Instance._decreaseSanity = RemoveInsanity;
        TryGetPostProcessingEffects();

        InvokeRepeating("DetectEnemies", 0f, 1f);
    }

    private void Update()
    {
        if (enableDebugMode)
        {
            CurrentInsanity = Mathf.Clamp(CurrentInsanity - Input.mouseScrollDelta.y, 0, 100);
            ApplyInsanity();
        }
    }

    private void TryGetPostProcessingEffects()
    {
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out colorAdjustments);
        globalVolume.profile.TryGet(out motionBlur);
        globalVolume.profile.TryGet(out vignette);
    }


    public void AddInsanity(float value)
    {
        CurrentInsanity = Mathf.Clamp(CurrentInsanity + value, 0, maxSanity);
        ApplyInsanity();
    }
    private void RemoveInsanity()
    {
        CurrentInsanity = Mathf.Clamp(CurrentInsanity - offerSanityReduction, 0, maxSanity);
        ApplyInsanity();
    }

    public void RemoveInsanity(float value)
    {
        CurrentInsanity = Mathf.Clamp(CurrentInsanity - value, 0, maxSanity);
        ApplyInsanity();
    }

    private void ApplyInsanity()
    {
        fogController.SetFogPercentage(CurrentInsanity);
        cameraController.ApplyFov(CurrentInsanity);
        ApplyPostProcessingEffects();
    }

    public IEnumerator HitPlayer(EnemyController enemy)
    {
        AddInsanity(hitInsanityPoints);
        ApplyInsanity();

        enemy.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);

        enemy.gameObject.SetActive(true);
        enemy.transform.position = enemy.points[0];
    }

    private void ApplyPostProcessingEffects()
    {
        float insanityFactor = CurrentInsanity / maxSanity;
        chromaticAberration.intensity.value = Mathf.Lerp(0, maxChromaticAberration, insanityFactor);
        motionBlur.intensity.value = Mathf.Lerp(0, maxMotionBlur, insanityFactor);
        colorAdjustments.saturation.value = Mathf.Lerp(0, maxDesaturation, insanityFactor);

        float vignetteIntensity = CurrentInsanity / 100f * maxVignetteIntensity;
        vignette.intensity.value = vignetteIntensity;

        foreach (Renderer renderer in FindObjectsOfType<Renderer>())
        {
            if (((1 << renderer.gameObject.layer) & exclusionLayer.value) != 0)
            {
                renderer.sharedMaterial.SetFloat("_Saturation", 1.0f);
            }
        }
    }

    private void DetectEnemies()
    {
        if (Camera.main == null) return;

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) > enemyDetectionRange) continue;

            if (IsObjectVisible(enemy.gameObject))
            {
                AddInsanity(enemyDetectionInsanityPoints);
                ApplyInsanity();
                return;
            }
        }
    }

    private bool IsObjectVisible(GameObject obj)
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
}
