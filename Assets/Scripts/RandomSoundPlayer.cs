using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private List<AudioClip> normalSoundClips = new ();
    [SerializeField] private List<AudioClip> insaneSoundClips = new ();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;
    [SerializeField] private bool loopRandomly = false;

    private Coroutine loopCoroutine;

    bool isInsane = false;

    private void Start()
    {
        Invoke("StartLoop", minDelay);
    }

    private void StartLoop()
    {
        if (loopRandomly && normalSoundClips.Count > 0)
        {
            loopCoroutine = StartCoroutine(PlayRandomLoop(false));
        }
    }

    public void PlayRandomSound(List<AudioClip> soundClips)
    {
        if (normalSoundClips.Count == 0 || audioSource == null)
            return;

        AudioClip clip = soundClips[Random.Range(0, soundClips.Count)];
        audioSource.PlayOneShot(clip);
    }

    public void StartRandomLoop(bool isInsane)
    { 
        if(this.isInsane != isInsane)
        {
            StopRandomLoop();
            this.isInsane = isInsane;
        }

        if (loopCoroutine == null)
            loopCoroutine = StartCoroutine(PlayRandomLoop(isInsane));
    }

    public void StopRandomLoop()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
    }

    private IEnumerator PlayRandomLoop(bool isInsane)
    {
        while (true)
        {
            PlayRandomSound(isInsane ? insaneSoundClips : normalSoundClips);
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }
}
