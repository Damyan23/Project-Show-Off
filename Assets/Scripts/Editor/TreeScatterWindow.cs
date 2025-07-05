using UnityEngine;
using UnityEditor;

public class TreeScatterWindow : EditorWindow
{
    // — User‐set in the window:
    private GameObject areaObject;       // must have a Collider
    private GameObject treePrefab;       // your tree model prefab
    private int treeCount = 100;
    private float minYOffset = 0.5f;     // how far above bounds.max to raycast from
    private Vector2 scaleRange = Vector2.one;
    private bool randomRotation = true;

    [MenuItem("Window/Tree Scatter")]
    public static void ShowWindow()
    {
        GetWindow<TreeScatterWindow>("Tree Scatter");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Area & Tree Prefab", EditorStyles.boldLabel);

        areaObject = (GameObject)EditorGUILayout.ObjectField("Area (with Collider)", areaObject, typeof(GameObject), true);
        treePrefab = (GameObject)EditorGUILayout.ObjectField("Tree Prefab", treePrefab, typeof(GameObject), false);

        treeCount = EditorGUILayout.IntField("Number of Trees", treeCount);
        minYOffset = EditorGUILayout.FloatField("Raycast Height Offset", minYOffset);

        EditorGUILayout.LabelField("Scale Range");
        scaleRange.x = EditorGUILayout.FloatField(" Min Scale", scaleRange.x);
        scaleRange.y = EditorGUILayout.FloatField(" Max Scale", scaleRange.y);

        randomRotation = EditorGUILayout.Toggle("Random Y Rotation", randomRotation);

        if (GUILayout.Button("Generate Trees"))
        {
            if (ValidateInputs())
                ScatterTrees();
        }
    }

    bool ValidateInputs()
    {
        if (areaObject == null || treePrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Assign both an area object and a tree prefab.", "OK");
            return false;
        }
        if (areaObject.GetComponent<Collider>() == null)
        {
            EditorUtility.DisplayDialog("Error", "Area object needs a Collider component to define bounds.", "OK");
            return false;
        }
        return true;
    }

    void ScatterTrees()
    {
        var col = areaObject.GetComponent<Collider>();
        Bounds b = col.bounds;                                                // world‐space box of the collider :contentReference[oaicite:2]{index=2}

        var parentGO = new GameObject("ScatteredTrees");
        parentGO.transform.parent = areaObject.transform;

        for (int i = 0; i < treeCount; i++)
        {
            // pick a random XZ inside the bounds
            float x = Random.Range(b.min.x, b.max.x);                         // Random.Range for floats :contentReference[oaicite:3]{index=3}
            float z = Random.Range(b.min.z, b.max.z);

            // start raycast a bit above the top of the box
            Vector3 origin = new Vector3(x, b.max.y + minYOffset, z);
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit))
            {
                // instantiate at the hit point
                var tree = (GameObject)PrefabUtility.InstantiatePrefab(treePrefab);
                tree.transform.position = hit.point;

                // random rotation
                if (randomRotation)
                    tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                // random scale
                float s = Random.Range(scaleRange.x, scaleRange.y);
                tree.transform.localScale = Vector3.one * s;

                tree.transform.parent = parentGO.transform;
            }
        }

        // focus on the new parent for convenience
        Selection.activeGameObject = parentGO;
    }
}
