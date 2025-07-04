using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float roamingSpeed = 2.5f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] float rotationSpeed = 1f;

    [Header("References")]
    private Transform playerT;

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
            //Move toward player
            agent.speed = chaseSpeed;
            Vector3 dirToPlayer = Vector3.Normalize(playerT.position - transform.position);
            dirToPlayer.y = 0f;
            if(dirToPlayer.magnitude > Mathf.Epsilon)
            {
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            agent.SetDestination(playerT.position);
        }
        else
        {
            //Move toward next point
            agent.speed = roamingSpeed;
            Vector3 dirToDestination = Vector3.Normalize(destination - transform.position);
            dirToDestination.y = 0f;
            if (dirToDestination.magnitude > Mathf.Epsilon)
            {
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(dirToDestination);
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
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

    public bool PlayerInRange()
    {
        if (playerT == null) return false;

        return Vector3.Distance(transform.position, playerT.position) < detectionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
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

        if(movementBehaviour == null)
        {
            if (!TryGetComponent(out movementBehaviour)) return;
        }
        movementBehaviour.DrawPath();
    }
}
