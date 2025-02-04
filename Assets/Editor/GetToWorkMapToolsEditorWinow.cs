using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.PackageManager.UI;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using System.Threading.Tasks;
using System.IO;

public class GetToWorkMapToolsEditorWinow : EditorWindow
{

    [MenuItem("Get To Work Tools/Map Tools/Map Tools Window")]
    public static void ShowWindow()
    {
        GetToWorkMapToolsEditorWinow window = GetWindow<GetToWorkMapToolsEditorWinow>();
        window.titleContent = new GUIContent("Get To Work Map Tools");
        window.Show();
    }

    [MenuItem("Get To Work Tools/Map Tools/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.gameObject.name == "Preview")
            {
                renderer.enabled = false;
            }

            if (renderer.gameObject.CompareTag("Finish"))
            {
                renderer.enabled = false;
            }
        }

        Debug.Log("Building Asset Bundles");

        System.IO.Directory.CreateDirectory("Assets/AssetBundles");
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        Debug.Log("Done Building Asset Bundles! Outputted to Assets/AssetBundles");

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform.parent.gameObject.CompareTag("Respawn"))
            {
                renderer.enabled = true;
            }

            if (renderer.gameObject.CompareTag("Finish"))
            {
                renderer.enabled = true;
            }

        }
    }

    static AddRequest Request;

    [MenuItem("Get To Work Tools/Map Tools/Install Probuilder")]
    static void ProbuilderSetup()
    {
        Request = Client.Add("com.unity.probuilder");
        EditorApplication.update += Progress;
    }

    static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
                Debug.Log("Installed: " + Request.Result.packageId);
            else if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= Progress;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Tools", EditorStyles.boldLabel);

        // Map-Metadata
        GUILayout.Label("Map Metadata");

        string MapName = "Map Name";
        string MapAuthor = "Map Author";

        MapName = EditorGUILayout.TextField(MapName);
        MapAuthor = EditorGUILayout.TextField(MapAuthor);

        // Update metadata
        MapData.Instance.SetData(MapName, MapAuthor);

        if (GUILayout.Button("Create Map Data"))
        {
            TextAsset data = new TextAsset("\nMap Name: " + MapName + "\nMap Author: " + MapAuthor);
            AssetDatabase.CreateAsset(data, "Assets/map.txt");
            Debug.Log(AssetDatabase.GetAssetPath(data));
            AssetDatabase.Refresh();
        }



        if (GUILayout.Button("Open Template Scene"))
        {
            Debug.Log("Open Template");
            EditorSceneManager.OpenScene("Assets/G2WTools/MapTools/TemplateScene.unity");
        }
        Vector3 spawnpointPos = EditorGUILayout.Vector3Field("Spawnpoint Position", Vector3.zero);
        if (GUILayout.Button("Create Spawnpoint"))
        {
            Debug.Log("Create Spawnpoint");
            GameObject spawnPoint = GameObject.Instantiate(Resources.Load<GameObject>("MapTools/Spawn"));
            spawnPoint.transform.position = spawnpointPos;
            spawnPoint.name = "Spawnpoint"; // make sure it doesn't have "(Clone)" in the name

            if(GameObject.Find("Map") != null)
            {
                spawnPoint.transform.parent = GameObject.Find("Map").transform;
            }
        }

        Vector3 respawnTriggerPos = EditorGUILayout.Vector3Field("Respawn Trigger Position", Vector3.zero);
        if (GUILayout.Button("Create Respawn Trigger"))
        {
            Debug.Log("Create Respawn");
            GameObject Respawn = GameObject.Instantiate(Resources.Load<GameObject>("MapTools/Respawn"));
            Respawn.transform.position = respawnTriggerPos;
            Respawn.name = "Respawn"; // make sure it doesn't have "(Clone)" in the name

            if (GameObject.Find("Map") != null)
            {
                Respawn.transform.parent = GameObject.Find("Map").transform;
            }
        }
    }
}

public class MapData
{
    private static MapData instance;
    public static MapData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MapData();
            }
            return instance;
        }
    }

    public string name;
    public string author;

    private MapData() { } // Private constructor to prevent instantiation

    public void SetData(string name, string author)
    {
        this.name = name;
        this.author = author;
    }
}

