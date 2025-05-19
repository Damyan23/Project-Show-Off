using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<Vector3> points;
    private int currentPointIndex;

    [HideInInspector] public float posY = 1f;

    [Header("Stats")]
    [SerializeField] float roamingSpeed = 2.5f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float detectionRadius = 10f;

    [Header("References")]
    private Transform playerT;

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

        detectedPlayer = Vector3.Distance(transform.position, playerT.position) < detectionRadius;

        if (detectedPlayer)
        {
            //Move toward player
            Vector3 dirToPlayer = Vector3.Normalize(playerT.position - transform.position);
            transform.Translate(dirToPlayer * Time.deltaTime * chaseSpeed);
        }
        else
        {
            //Move toward next point
            Vector3 dirToNextPoint = Vector3.Normalize(points[currentPointIndex] - transform.position);
            transform.Translate(dirToNextPoint * Time.deltaTime * roamingSpeed);

            if(Vector3.Distance(transform.position, points[currentPointIndex]) < 0.1f)
            {
                currentPointIndex++;
                if (currentPointIndex >= points.Count) currentPointIndex = 0;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.name == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.StartCoroutine(player.HitPlayer(this));
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
