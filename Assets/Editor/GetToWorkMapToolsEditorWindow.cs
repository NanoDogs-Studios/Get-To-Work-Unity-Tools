using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace Nanodogs.GetToWork.MapTools
{
    /// <summary>
    /// Editor window for Get To Work Map Tools.
    /// This window provides options to build asset bundles, install ProBuilder, and create map metadata.
    /// </summary>

    public class GetToWorkMapToolsEditorWindow : EditorWindow
    {
        private string mapName = "Map Name";
        private string mapAuthor = "Map Author";
        private Vector3 spawnpointPos = Vector3.zero;
        private Vector3 respawnTriggerPos = Vector3.zero;
        static TextAsset dataasset;

        [MenuItem("Get To Work Tools/Map Tools/Map Tools Window")]
        public static void ShowWindow()
        {
            GetToWorkMapToolsEditorWindow window = GetWindow<GetToWorkMapToolsEditorWindow>();
            window.titleContent = new GUIContent("Get To Work Map Tools");
            window.Show();
        }

        [MenuItem("Get To Work Tools/Map Tools/Build Asset Bundles")]
        static void BuildAllAssetBundles()
        {
            MeshRenderer[] renderers = GameObject.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);

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

            // Use EditorPrefs or Serialized Variables to retain values
            mapName = EditorGUILayout.TextField("Map Name", mapName);
            mapAuthor = EditorGUILayout.TextField("Map Author", mapAuthor);

            // Update metadata
            MapData.Instance.SetData(mapName, mapAuthor);

            if (GUILayout.Button("Create Map Data"))
            {
                dataasset = new TextAsset($"\nMap Name: {mapName}\nMap Author: {mapAuthor}");
                AssetDatabase.CreateAsset(dataasset, $"Assets/AssetBundles/{mapName}.txt");
                Debug.Log(AssetDatabase.GetAssetPath(dataasset));
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Open Template Scene"))
            {
                Debug.Log("Open Template");
                EditorSceneManager.OpenScene("Assets/G2WTools/MapTools/TemplateScene.unity");
            }

            spawnpointPos = EditorGUILayout.Vector3Field("Spawnpoint Position", spawnpointPos);
            if (GUILayout.Button("Create Spawnpoint"))
            {
                Debug.Log("Create Spawnpoint");
                GameObject spawnPoint = GameObject.Instantiate(Resources.Load<GameObject>("MapTools/Spawn"));
                spawnPoint.transform.position = spawnpointPos;
                spawnPoint.name = "Spawnpoint"; // make sure it doesn't have "(Clone)" in the name

                if (GameObject.Find("Map") != null)
                {
                    spawnPoint.transform.parent = GameObject.Find("Map").transform;
                }
            }

            respawnTriggerPos = EditorGUILayout.Vector3Field("Respawn Trigger Position", respawnTriggerPos);
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
}

