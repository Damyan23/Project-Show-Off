using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyPathBehaviour))]
public class PathEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyPathBehaviour path = (EnemyPathBehaviour)target;


        for(int i = 0; i < path.points.Count; i++)
        {
            //Get handle position
            Vector3 oldPosition = path.points[i];
            Vector3 newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);

            //If difference is big enough, update the list
            if(Vector3.Distance(oldPosition, newPosition) > 0.001f) path.points[i] = newPosition;

            if (path.points.Count < 2) continue;

            //Draw lines between all points
            Handles.color = Color.white;
            if(i < path.points.Count - 1)
            {
                Handles.DrawLine(path.points[i], path.points[i + 1]);
            }
            else
            {
                Handles.DrawLine(path.points[i], path.points[0]);
            }
        }
    }
}
