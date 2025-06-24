using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovementBehaviour : MonoBehaviour
{
    public abstract Vector3 GetStartPosition();
    public abstract void UpdateDestination();
    public abstract Vector3 GetDestination();
    public abstract void DrawPath();
}
