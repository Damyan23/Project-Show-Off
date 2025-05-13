using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class PathEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyController enemyController = (EnemyController)target;

        for(int i = 0; i < enemyController.points.Count; i++)
        {
            //Get handle position
            Vector3 oldPosition = enemyController.points[i];
            Vector3 newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
            newPosition.y = enemyController.posY;

            //If difference is big enough, update the list
            if(Vector3.Distance(oldPosition, newPosition) > 0.001f) enemyController.points[i] = newPosition;

            //Draw lines between all points
            Handles.color = Color.white;
            if(i < enemyController.points.Count - 1)
            {
                Handles.DrawLine(enemyController.points[i], enemyController.points[i + 1]);
            }
            else
            {
                Handles.DrawLine(enemyController.points[i], enemyController.points[0]);
            }
        }
    }
}
