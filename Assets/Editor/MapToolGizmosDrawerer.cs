using UnityEditor;
using UnityEngine;
using Nanodogs.GetToWork.MapTools; // Make sure this namespace matches your editor window

[InitializeOnLoad]
public static class MapToolGizmosDrawer
{
    static MapToolGizmosDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!GetToWorkMapToolsEditorWindow.IsWindowOpen) return;

        Handles.color = Color.green;
        EditorGUI.BeginChangeCheck();
        Vector3 newSpawnPos = Handles.PositionHandle(GetToWorkMapToolsEditorWindow.GizmosSpawnPointPos, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sceneView, "Move Spawnpoint");
            GetToWorkMapToolsEditorWindow.GizmosSpawnPointPos = newSpawnPos;
            SceneView.RepaintAll();
        }
        Handles.Label(newSpawnPos + Vector3.up * 1.2f, "Spawnpoint");

        Handles.color = Color.red;
        EditorGUI.BeginChangeCheck();
        Vector3 newRespawnPos = Handles.PositionHandle(GetToWorkMapToolsEditorWindow.GizmosRespawnTriggerPos, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sceneView, "Move Respawn Trigger");
            GetToWorkMapToolsEditorWindow.GizmosRespawnTriggerPos = newRespawnPos;
            SceneView.RepaintAll();
        }
        Handles.Label(newRespawnPos + Vector3.up * 1.2f, "Respawn Trigger");
    }

}
