using Adobe.Substance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemySoundController : MonoBehaviour
{
    private float soundFadeT = 0f;
    [SerializeField] private AudioClip chaseSound;
    [SerializeField] private float fadeTime = 5f;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MixerSettings[] mixerSettings;

    private void Start()
    {
        soundFadeT = 0f;
    }

    void Update()
    {
        if (AnyEnemyInRange())
        {
            if (!audioSource.isPlaying) audioSource.PlayOneShot(chaseSound);

            soundFadeT = Mathf.Clamp(soundFadeT + Time.deltaTime / fadeTime, 0f, 1f);
        }
        else
        {
            soundFadeT = Mathf.Clamp(soundFadeT - Time.deltaTime / fadeTime, 0f, 1f);

            if (soundFadeT < Mathf.Epsilon) audioSource.Stop();
        }

        MixerSettings.ApplySettings(mixerSettings, mixer, soundFadeT);
    }

    private bool AnyEnemyInRange()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            if (enemy.PlayerInRange()) return true;
        }

        return false;
    }
}
