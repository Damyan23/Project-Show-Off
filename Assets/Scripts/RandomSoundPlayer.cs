using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private List<AudioClip> soundClips = new ();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;
    [SerializeField] private bool loopRandomly = false;

    private Coroutine loopCoroutine;

    private void Start()
    {
        if (loopRandomly && soundClips.Count > 0)
        {
            loopCoroutine = StartCoroutine(PlayRandomLoop());
        }
    }

    public void PlayRandomSound()
    {
        if (soundClips.Count == 0 || audioSource == null)
            return;

        AudioClip clip = soundClips[Random.Range(0, soundClips.Count)];
        audioSource.PlayOneShot(clip);
    }

    public void StartRandomLoop()
    {
        if (loopCoroutine == null)
            loopCoroutine = StartCoroutine(PlayRandomLoop());
    }

    public void StopRandomLoop()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
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
}
