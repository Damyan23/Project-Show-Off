using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

[ExecuteInEditMode]
public class TreeBrush : EditorWindow
{
    public GameObject treePrefab;
    public float brushSize = 5f;
    public float treeRadius = 1f;
    public int treeCount = 10;
    public LayerMask hitMask;

    [MenuItem("Tools/Tree Brush")]
    public static void ShowWindow() => GetWindow<TreeBrush>("Tree Brush");

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        Transform treeParent = GameObject.Find("Tree Parent").transform;

        Handles.color = Color.blue;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit mouseHit, 1000f);
        Handles.DrawWireDisc(mouseHit.point, Vector3.up, brushSize);

        if (e.type == EventType.MouseDown && e.button == 0 && treePrefab)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitMask))
            {
                if (e.shift)
                {
                    Collider[] hits = Physics.OverlapSphere(hit.point, brushSize);
                    foreach (Collider c in hits)
                    {  
                        if (c.transform.CompareTag("Tree"))
                        {
                            DestroyImmediate(c.gameObject);
                        }
                    }
                }

                else
                {
                    List<Vector3> placedTreePositions = new();

                    for (int i = 0; i < treeCount; i++)
                    {
                        Vector3 offset = Random.insideUnitCircle * brushSize;
                        Vector3 pos = hit.point + new Vector3(offset.x, 0, offset.y);
                        if (Physics.Raycast(pos + Vector3.up * 50, Vector3.down, out RaycastHit groundHit, 100f, hitMask))
                        {
                            Collider[] trees = Physics.OverlapSphere(groundHit.point, treeRadius);

                            bool placeTree = true;
                            foreach (Collider c in trees)
                            {
                                if (c.transform.CompareTag("Tree")) 
                                {
                                    placeTree = false;
                                    break;
                                }
                            }

                            foreach (Vector3 existing in placedTreePositions)
                            {
                                if (Vector3.Distance(existing, groundHit.point) < treeRadius)
                                {
                                    placeTree = false;
                                    break;
                                }
                            }

                            if (!placeTree) continue;

                            GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(treePrefab);
                            float randomRotation = Random.Range(0f, 360f);
                            tree.transform.Rotate(new Vector3(0f, randomRotation, 0f));
                            tree.transform.parent = treeParent;
                            tree.transform.position = groundHit.point;
                            //tree.transform.up = groundHit.normal; // optional alignment
                            Undo.RegisterCreatedObjectUndo(tree, "Place Tree");

                            placedTreePositions.Add(groundHit.point);


                        }
                    }
                    e.Use(); // consume the click
                }
                }
            }
        }

        void OnGUI()
        {
            treePrefab = (GameObject)EditorGUILayout.ObjectField("Tree Prefab", treePrefab, typeof(GameObject), false);
            brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 0.1f, 50f);
            treeRadius = EditorGUILayout.Slider("Tree Radius", treeRadius, 0.1f, 50f);
            treeCount = EditorGUILayout.IntSlider("Trees per Click", treeCount, 1, 100);
            hitMask = LayerMaskField("Hit Layer Mask", hitMask);
        }

        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            var layers = InternalEditorUtility.layers;
            var layerNumbers = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                layerNumbers[i] = LayerMask.NameToLayer(layers[i]);
            var maskWithoutEmpty = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);
            int mask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }
    }
