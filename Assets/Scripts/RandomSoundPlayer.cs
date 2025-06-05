using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Audio;

public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private List<AudioClip> normalSoundClips = new ();
    [SerializeField] private List<AudioClip> insaneSoundClips = new ();
    [SerializeField] private float insaneThreshold = 75f;
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;
    [SerializeField] private bool loopRandomly = false;

    [Header("Mixer Settings")]
    [SerializeField] private List<MixerSettings> settings;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer mixer;

    private Coroutine loopCoroutine;

    private List<AudioClip> activeClips;

    private AudioMixerSnapshot normalSnapshot;
    private AudioMixerSnapshot insaneSnapshot;

    bool isInsane = false;



    private void Start()
    {
        Invoke("StartLoop", minDelay);

        activeClips = normalSoundClips;

        normalSnapshot = mixer.FindSnapshot("Normal");
        insaneSnapshot = mixer.FindSnapshot("Insane");
    }

    private void StartLoop()
    {
        if (loopRandomly && activeClips.Count > 0)
        {
            loopCoroutine = StartCoroutine(PlayRandomLoop());
        }
    }

    private void PlayRandomSound()
    {
        if (activeClips.Count == 0 || audioSource == null)
            return;

        audioSource.PlayOneShot(activeClips[Random.Range(0, activeClips.Count)]);
    }

    private void StartRandomLoop()
    { 
        if (loopCoroutine == null)
            loopCoroutine = StartCoroutine(PlayRandomLoop());
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
        //Set the correct clips to be played
        activeClips = insanity > insaneThreshold ? insaneSoundClips : normalSoundClips;

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
