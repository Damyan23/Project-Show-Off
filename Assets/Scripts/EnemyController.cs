using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyController : MonoBehaviour
{
    public List<Vector3> points;
    private int currentPointIndex;

    [Header("Stats")]
    [SerializeField] float roamingSpeed = 2.5f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float detectionRadius = 10f;

    [Header("Sound Settings")]
    [SerializeField] private MixerSettings[] mixerSettings;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float fadeTime;

    [Header("References")]
    private Transform playerT;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chaseSound;

    private bool detectedPlayer;

    private void Start()
    {
        currentPointIndex = 0;
        transform.position = points[currentPointIndex];
        detectedPlayer = false;
    }

    private void Update()
    {
        // If player is not assigned and we can find the player, assign it
        if (playerT == null && GameObject.FindGameObjectWithTag("Player")) playerT = GameObject.FindGameObjectWithTag("Player").transform;

        if (Vector3.Distance(transform.position, playerT.position) < detectionRadius)
        {
            if (!detectedPlayer)
            {
                StopCoroutine("FadeSound");
                StartCoroutine(FadeSound(true));
                detectedPlayer = true;
            }
        }
        else
        {
            if (detectedPlayer)
            {
                StopCoroutine("FadeSound");
                StartCoroutine(FadeSound(false));
                detectedPlayer = false;
            }
        }


        if (detectedPlayer)
        {

            //Move toward player
            Vector3 dirToPlayer = Vector3.Normalize(playerT.position - transform.position);
            transform.Translate(chaseSpeed * Time.deltaTime * dirToPlayer, Space.World);
            dirToPlayer.y = 0f;
            transform.rotation = Quaternion.LookRotation(dirToPlayer);
        }
        else
        {
            //Move toward next point
            Vector3 dirToNextPoint = Vector3.Normalize(points[currentPointIndex] - transform.position);
            transform.Translate(roamingSpeed * Time.deltaTime * dirToNextPoint, Space.World);
            dirToNextPoint.y = 0f;
            transform.rotation = Quaternion.LookRotation(dirToNextPoint);

            if (Vector3.Distance(transform.position, points[currentPointIndex]) < 0.1f)
            {
                currentPointIndex++;
                if (currentPointIndex >= points.Count) currentPointIndex = 0;
            }
        }

    }


    private IEnumerator FadeSound(bool fadeIn)
    {
        float startTime = Time.time;
        if (fadeIn) audioSource.PlayOneShot(chaseSound);

        while (Time.time - startTime < fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            if (!fadeIn) t = 1f - t;
            MixerSettings.ApplySettings(mixerSettings, mixer, t);
            yield return null;
        }

        if (!fadeIn) audioSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player")
        {
            SanityController sanityController = other.GetComponent<SanityController>();
            StartCoroutine(sanityController.HitPlayer(this));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnValidate()
    {
        transform.position = points[0];
    }
}
