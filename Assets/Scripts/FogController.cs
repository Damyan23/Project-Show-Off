using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogController : MonoBehaviour
{

    [Header("Fog Settings")]
    [SerializeField] float minFogDistance;
    [SerializeField] float maxFogDistance;

    [SerializeField, Range(0, 100f)] float fogPercentage = 0f;

    [Header("References")]
    [SerializeField] Volume globalFogVolume;

    private Fog globalFog;

    private void Awake()
    {
        globalFogVolume.profile.TryGet<Fog>(out globalFog);
        globalFog.meanFreePath.value = maxFogDistance;
    }

    public void SetFogValue(float newFogValue)
    {
        newFogValue = Mathf.Min(Mathf.Max(minFogDistance, newFogValue), maxFogDistance);
        globalFog.meanFreePath.value = newFogValue;
    }

    public void SetFogPercentage(float percentage)
    {
        fogPercentage = percentage;
        float diff = maxFogDistance - minFogDistance;
        float factor = percentage / 100f;
        float newFogValue = maxFogDistance - factor * diff;
        globalFog.meanFreePath.value = newFogValue;
    }

    private void OnValidate()
    {
        minFogDistance = Mathf.Max(1, minFogDistance);
        maxFogDistance = Mathf.Max(minFogDistance + 1, maxFogDistance);

        if (globalFogVolume == null) return;

        if (globalFogVolume.profile.TryGet<Fog>(out globalFog)) SetFogPercentage(fogPercentage);
        
    }
}
