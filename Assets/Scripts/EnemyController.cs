using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] float detectionRadius;
    [SerializeField] float roamingRadius;
    [SerializeField] float roamingSpeed;
    [SerializeField] float chaseSpeed;

    [Header("References")]
    [SerializeField] Transform player;

    private bool detectedPlayer;

    private void Start()
    {
        detectedPlayer = false;
    }

    private void Update()
    {
        detectedPlayer = Vector3.Distance(transform.position, player.position) < detectionRadius;

        if (detectedPlayer)
        {
            Vector3 playerDir = Vector3.Normalize(player.position - transform.position);
            transform.Translate(playerDir * chaseSpeed * Time.deltaTime);
        }
        else
        {
            Vector2 circle = new Vector2(Mathf.Cos(Time.time / 10f * roamingSpeed), Mathf.Sin(Time.time / 10f * roamingSpeed)) * roamingRadius;
            transform.position = new Vector3(circle.x, 1, circle.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, roamingRadius);
    }
}
