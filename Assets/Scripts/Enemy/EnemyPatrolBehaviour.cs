using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolBehaviour : EnemyMovementBehaviour
{
    [SerializeField] private Vector3 patrolPosition;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float waitTimeBetweenPoints = 2.5f;

    private Vector3 currentDestination;
    private bool canSetNewDestination = true;

    public override void DrawPath()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolPosition, patrolRadius);
    }

    public override Vector3 GetDestination()
    {
        return currentDestination;
    }

    public override Vector3 GetStartPosition()
    {
        return patrolPosition;
    }

    public override void UpdateDestination()
    {
        StartCoroutine(updateDestination());
    }

    private IEnumerator updateDestination()
    {
        if (!canSetNewDestination) yield break;

        canSetNewDestination = false;

        yield return new WaitForSeconds(waitTimeBetweenPoints);

        float rand = Random.Range(0f, Mathf.PI * 2f);
        Vector2 randomDirection = new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));
        float randomDst = Random.Range(0f, patrolRadius);
        Vector2 randomPoint = randomDirection * randomDst;

        Physics.Raycast(new Vector3(randomPoint.x, 50f, randomPoint.y), Vector3.down, out RaycastHit hit);
        currentDestination = hit.point;

        canSetNewDestination = true;
    }
}
