using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
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
    private NavMeshAgent agent;

    private EnemyMovementBehaviour movementBehaviour;
    private Vector3 destination;

    private void Start()
    {
        movementBehaviour = GetComponent<EnemyMovementBehaviour>();
        agent = GetComponent<NavMeshAgent>();   
        agent.speed = roamingSpeed; 

        transform.position = movementBehaviour.GetStartPosition();
        movementBehaviour.UpdateDestination();
        destination = movementBehaviour.GetDestination();
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
                if(!audioSource.isPlaying) StartCoroutine(FadeSound(true));
                detectedPlayer = true;
                agent.speed = chaseSpeed;
            }
        }
        else
        {
            if (detectedPlayer)
            {
                StopCoroutine("FadeSound");
                StartCoroutine(FadeSound(false));
                detectedPlayer = false;
                agent.speed = roamingSpeed;
            }
        }


        if (detectedPlayer)
        {
            //Move toward player
            Vector3 dirToPlayer = Vector3.Normalize(playerT.position - transform.position);
            dirToPlayer.y = 0f;
            if(dirToPlayer.magnitude > Mathf.Epsilon) transform.rotation = Quaternion.LookRotation(dirToPlayer);
            agent.SetDestination(playerT.position);
        }
        else
        {
            //Move toward next point
            Vector3 dirToDestination = Vector3.Normalize(destination - transform.position);
            dirToDestination.y = 0f;
            if (dirToDestination.magnitude > Mathf.Epsilon) transform.rotation = Quaternion.LookRotation(dirToDestination);
            SetDestination();

            if (Vector3.Distance(transform.position, destination) < 1f)
            {
                movementBehaviour.UpdateDestination();
                SetDestination();
            }
        }

    }

    private void SetDestination()
    {
        destination = movementBehaviour.GetDestination();
        agent.SetDestination(destination);
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
        return;

        if (other.transform.name == "Player")
        {
            SanityController sanityController = other.GetComponent<SanityController>();
            if(sanityController != null) StartCoroutine(sanityController.HitPlayer(this));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if(movementBehaviour == null) movementBehaviour = GetComponent<EnemyMovementBehaviour>();
        movementBehaviour.DrawPath();
    }
}
