using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogController : MonoBehaviour
{
    Volume fogVolume;
    Fog globalFog;
    LocalVolumetricFog localFog;

    [SerializeField] Transform player;
    [SerializeField] Transform localFogT;

    [SerializeField] float fogLowerBound;
    [SerializeField] float fogUpperBound;   

    private void Awake()
    {
        fogVolume = GetComponent<Volume>();
        fogVolume.profile.TryGet<Fog>(out globalFog);

        localFog = localFogT.GetComponent<LocalVolumetricFog>();

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

    private void OnValidate()
    {
        if(localFog == null) localFog = localFogT.GetComponent<LocalVolumetricFog>();
        Vector3 size = localFog.parameters.size;
        localFog.parameters.textureTiling = new Vector3(size.x * 0.04f, size.y * 0.1f, size.z * 0.04f);

        //Validate the bounds
        fogLowerBound = Mathf.Max(1, fogLowerBound);
        fogUpperBound = Mathf.Max(fogLowerBound + 1, fogUpperBound);

    }
}
