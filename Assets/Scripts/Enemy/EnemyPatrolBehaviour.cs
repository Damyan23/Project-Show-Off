using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolBehaviour : EnemyMovementBehaviour
{
    [SerializeField] private Vector3 patrolPosition;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float waitTimeBetweenPoints = 2.5f;

    private Vector3 currentDestination;
    private bool canSetDestination = true;

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
        patrolPosition = transform.position;
        NavMesh.SamplePosition(patrolPosition, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas);
        patrolPosition = hit.position;
        return hit.position;
    }

    public override void UpdateDestination()
    {
        if (canSetDestination)
        {
            StartCoroutine(updateDestination());
        }

    }

    private IEnumerator updateDestination()
    {
        canSetDestination = false;

        yield return new WaitForSeconds(waitTimeBetweenPoints);

        float rand = Random.Range(0f, Mathf.PI * 2f);
        Vector2 randomDirection = new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));
        float randomDst = Random.Range(0f, patrolRadius);
        Vector2 randomPoint = randomDirection * randomDst;

        Physics.Raycast(patrolPosition + new Vector3(randomPoint.x, 50f, randomPoint.y), Vector3.down, out RaycastHit hit, 75f, LayerMask.GetMask("Ground"));

        NavMesh.SamplePosition(hit.point, out NavMeshHit meshHit, patrolRadius, NavMesh.AllAreas);
        currentDestination = meshHit.position;

        canSetDestination = true;
    }
}
