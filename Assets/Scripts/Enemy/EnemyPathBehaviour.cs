using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathBehaviour : EnemyMovementBehaviour
{
    private int checkpointIndex = 0;
    public List<Vector3> points;

    public override void DrawPath()
    {
        //Drawing paths is being handled by the custom editor
    }

    public override Vector3 GetDestination()
    {
        NavMesh.SamplePosition(points[checkpointIndex], out NavMeshHit hit, 100f, NavMesh.AllAreas);
        return hit.position;
    }

    public override Vector3 GetStartPosition()
    {
        NavMesh.SamplePosition(points[0], out NavMeshHit hit, 100f, NavMesh.AllAreas);
        return hit.position;
    }

    public override void UpdateDestination()
    {
        checkpointIndex++;
        if (checkpointIndex >= points.Count) checkpointIndex = 0;
    }
}
