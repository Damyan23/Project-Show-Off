using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return points[checkpointIndex];
    }

    public override Vector3 GetStartPosition()
    {
        return points[0];
    }

    public override void UpdateDestination()
    {
        checkpointIndex++;
        if (checkpointIndex >= points.Count) checkpointIndex = 0;
    }
}
