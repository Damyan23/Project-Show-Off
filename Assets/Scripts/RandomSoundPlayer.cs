using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private List<SoundClip> soundClips;
    [SerializeField] private float insaneThreshold = 75f;
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;

    [Header("Mixer Settings")]
    [SerializeField] private List<MixerSettings> settings;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer mixer;

    private AudioMixerSnapshot normalSnapshot;
    private AudioMixerSnapshot insaneSnapshot;

    bool isInsane = false;

    private void Start()
    {
        Invoke("StartLoop", minDelay);

        normalSnapshot = mixer.FindSnapshot("Normal");
        insaneSnapshot = mixer.FindSnapshot("Insane");
    }

    private void StartLoop()
    {
        if (soundClips.Count > 0)
        {
            StartCoroutine(PlayRandomLoop());
        }
    }

    private void PlayRandomSound()
    {
        if (soundClips.Count == 0 || audioSource == null)
            return;

        SoundClip soundClip = GetRandomSound();
        if (soundClip == null) return;

        audioSource.PlayOneShot(soundClip.audioClip);
    }

    private SoundClip GetRandomSound()
    {
        float totalWeight = 0f;

        foreach (SoundClip clip in soundClips)
        {
            if (!isInsane && clip.insane) continue;
            totalWeight += clip.weight;
        }

        float rand = Random.Range(0f, totalWeight);
        float sumOfWeights = 0f;
        int i = -1;

        while(sumOfWeights < rand)
        {
            SoundClip clip = soundClips[i + 1];
            if (!isInsane && clip.insane)
            {
                i++;
                continue;
            }

            sumOfWeights += clip.weight;
            i++;
        }

        if (sumOfWeights == 0f) return null;

        //Debug.Log($"Total weight: {totalWeight}, Random number: {rand}, Chosen clip index: {i}");

        return soundClips[i];
    }

    private IEnumerator PlayRandomLoop()
    { 
        while (true)
        {
            PlayRandomSound();
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    public void ApplySFX(float insanity, float maxInsanity)
    {
        isInsane = insanity > insaneThreshold;

        //Apply sanity to all settings
        float t = insanity / maxInsanity;
        foreach(MixerSettings setting in settings)
        {
            mixer.SetFloat(setting.parameterName, Mathf.Lerp(setting.lowerValue, setting.upperValue, t));
        }
    }
}


//Class can be used to change audio mixer settings easily in the inspector
[System.Serializable]
public struct MixerSettings
{
    public string parameterName;
    public float lowerValue;
    public float upperValue;
}

[System.Serializable]
public class SoundClip
{
    public AudioClip audioClip;
    public float weight;
    public bool insane;
}
