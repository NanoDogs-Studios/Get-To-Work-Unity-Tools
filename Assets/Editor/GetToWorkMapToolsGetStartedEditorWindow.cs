using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace Nanodogs.GetToWork.MapTools
{
    /// <summary>
    /// Editor window for getting started with Get To Work Map Tools.
    /// This window provides options to read documentation, install ProBuilder, load a template scene,
    /// </summary>
    public class GetToWorkMapToolsGetStartedEditorWinow : EditorWindow
    {
        private string mapName = "Map Name";
        private string mapAuthor = "Map Author";

        private Texture2D bannerImage;
        bool showWindow = false;

        static TextAsset dataasset;

        [MenuItem("Get To Work Tools/Map Tools/Get Started", priority = 5)]
        public static void ShowWindow()
        {
            GetToWorkMapToolsGetStartedEditorWinow window = GetWindow<GetToWorkMapToolsGetStartedEditorWinow>();
            window.titleContent = new GUIContent("Getting Started (G2W Map Tools)");
            window.Show();
        }

        private void OnEnable()
        {
            // Load the banner image (make sure it's in a Resources folder)
            bannerImage = Resources.Load<Texture2D>("MapTools/gtwtoolsbanner");
        }

        private void OnGUI()
        {

            #region styles
            GUIStyle centered = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            GUIStyle centeredlink = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                hover = { textColor = Color.blue },
            };

            GUIStyle centeredButtonBig = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                hover = { textColor = Color.cyan },
                fontSize = 15,
                fixedHeight = 50
            };

            GUIStyle centeredBigger = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
            #endregion

            if (bannerImage != null)
            {
                Rect bannerRect = new Rect(0, 0, position.width, 100);
                GUI.DrawTexture(bannerRect, bannerImage, ScaleMode.ScaleAndCrop);
                GUIStyle textStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 22,
                    normal = { textColor = Color.black },
                    hover = { textColor = Color.black }
                };
                GUI.Label(bannerRect, "Getting Started With G2W Map Tools", textStyle);
                GUILayout.Space(110);
            }
            else
            {
                GUILayout.Label("(Banner image not found. Place 'banner.png' in Resources.)", EditorStyles.centeredGreyMiniLabel);
            }

            GUILayout.Label("Getting Started!", centered);

            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox("Welcome to the Get To Work Map Tools! Credits: LaymGlitched - Creating Map Tools & G2WMP, Isto Inc. - For Creating G2W, Yyk - Helping with G2WMP", MessageType.Info);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Read the Docs", centeredButtonBig, GUILayout.Height(50), GUILayout.ExpandWidth(true)))
            {
                Application.OpenURL("https://laymglitched.gitbook.io/get-to-work-modding");
            }

            if (GUILayout.Button("Install ProBuilder", centeredButtonBig, GUILayout.Height(50), GUILayout.ExpandWidth(true)))
            {
                ProbuilderSetup();
            }

            if (GUILayout.Button("Load the Template Scene", centeredButtonBig, GUILayout.Height(50), GUILayout.ExpandWidth(true)))
            {
                EditorSceneManager.OpenScene("Assets/G2WTools/MapTools/TemplateScene.unity");
            }

            if (GUILayout.Button("Create Map Data", centeredButtonBig, GUILayout.Height(50), GUILayout.ExpandWidth(true)))
            {
                showWindow = true;
            }
            if (showWindow)
            {
                BeginWindows();
                GUILayout.Window(0, new Rect(0, 0, 400, 200), DoWindow, "Map Data");
                EndWindows();
            }

            GUILayout.Label("Having issues or have a question?", centered);
            if (GUILayout.Button("Make a Github Issue", centeredlink, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {
                Application.OpenURL("https://github.com/NanoDogs-Studios/Get-To-Work-Unity-Tools/issues/new");
            }
        }

        void DoWindow(int unusedWindowID)
        {
            mapName = EditorGUILayout.TextField("Map Name", mapName);
            mapAuthor = EditorGUILayout.TextField("Map Author", mapAuthor);
            this.Repaint();

            // Update metadata
            MapData.Instance.SetData(mapName, mapAuthor);

            if (GUILayout.Button("Create Map Data (.txt)"))
            {
                if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(mapAuthor))
                {
                    EditorUtility.DisplayDialog("Error", "Please enter both Map Name and Map Author.", "OK");
                    return;
                }
                dataasset = new TextAsset($"\nMap Name: {mapName}\nMap Author: {mapAuthor}\n");
                AssetDatabase.CreateAsset(dataasset, $"Assets/AssetBundles/{mapName}.txt");
                Debug.Log(AssetDatabase.GetAssetPath(dataasset));
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Close"))
            {
                showWindow = false; // Close the window
            }

            GUI.DragWindow();
        }


        static AddRequest Request;

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
    }
}