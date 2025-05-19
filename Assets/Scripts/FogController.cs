using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogController : MonoBehaviour
{
    Fog globalFog;

    [Header("Fog Settings")]
    [SerializeField] float fogLowerBound;
    [SerializeField] float fogUpperBound;

    [SerializeField, Range(0, 100f)] float fogPercentage = 0f;

    [Header("References")]
    [SerializeField] Volume globalFogVolume;
    [SerializeField] LocalVolumetricFog localFog;


    private void Awake()
    {
        globalFogVolume.profile.TryGet<Fog>(out globalFog);

        globalFog.meanFreePath.value = fogUpperBound;
    }

    private void Update()
    {
        float newFogValue = globalFog.meanFreePath.value - Input.mouseScrollDelta.y * 0.5f;
        SetFogValue(newFogValue);
    }

    public void SetFogValue(float newFogValue)
    {
        newFogValue = Mathf.Min(Mathf.Max(fogLowerBound, newFogValue), fogUpperBound);
        globalFog.meanFreePath.value = newFogValue;
    }

    public void SetFogPercentage(float percentage)
    {
        fogPercentage = percentage;
        float diff = fogUpperBound - fogLowerBound;
        float factor = percentage / 100f;
        float newFogValue = fogUpperBound - factor * diff;
        globalFog.meanFreePath.value = newFogValue;
    }

    private void OnValidate()
    {
        //Validate the bounds
        if (globalFog != null)
        {
            globalFogVolume.profile.TryGet<Fog>(out globalFog);
            fogLowerBound = Mathf.Max(1, fogLowerBound);
            fogUpperBound = Mathf.Max(fogLowerBound + 1, fogUpperBound);
            globalFog.meanFreePath.value = Mathf.Min(globalFog.meanFreePath.value, fogUpperBound);

            SetFogPercentage(fogPercentage);
        }
    }
}
