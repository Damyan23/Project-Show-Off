using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonologueController))]
public class MonologueControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws default serialized fields

        MonologueController controller = (MonologueController)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Dialogue Triggers", EditorStyles.boldLabel);

        GUI.enabled = Application.isPlaying; // Only enable buttons during Play Mode

        if (GUILayout.Button("Trigger Forest General"))
            controller.TriggerForestGeneral();

        if (GUILayout.Button("Trigger Altar Look General"))
            controller.TriggerAltarLookGeneral();

        if (GUILayout.Button("Trigger Altar Item Placed General"))
            controller.TriggerAltarItemPlacedGeneral();

        if (GUILayout.Button("Trigger Sanity High"))
            controller.TriggerSanityHigh();

        if (GUILayout.Button("Trigger Game Over"))
            controller.TriggerGameOver();

        if (GUILayout.Button("Trigger White Women Encounter"))
            controller.TriggerWhiteWomenEncounter();

        GUI.enabled = true; // Reset GUI state
    }
}
