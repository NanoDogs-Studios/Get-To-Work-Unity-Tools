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

public class GetToWorkMapToolsGetStartedEditorWinow : EditorWindow
{
    private Texture2D bannerImage;
    bool showWindow = false;

    [InitializeOnLoadMethod]
    [MenuItem("Get To Work Tools/Map Tools/Get Started")]
    public static void ShowWindow()
    {
        const string k_ProjectOpened = "ProjectOpened";

        if (!SessionState.GetBool(k_ProjectOpened, false) && EditorApplication.isPlayingOrWillChangePlaymode == false)
        {
            SessionState.SetBool(k_ProjectOpened, true);

            GetToWorkMapToolsGetStartedEditorWinow window = GetWindow<GetToWorkMapToolsGetStartedEditorWinow>();
            window.titleContent = new GUIContent("Getting Started (G2W MTools)");
            window.Show();
        }
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
        if(GUILayout.Button("Install ProBuilder", centeredButtonBig, GUILayout.Height(50), GUILayout.ExpandWidth(true)))
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
            GUILayout.Window(0, new Rect(0, 0, 200, 200), DoWindow, "Map Data");
            EndWindows();
        }
    }

    void DoWindow(int unusedWindowID)
    {
        string MapName = EditorGUILayout.TextField("Map Name");
        string MapAuthor = EditorGUILayout.TextField("Map Author");
        this.Repaint();

        // Update metadata
        MapData.Instance.SetData(MapName, MapAuthor);

        if (GUILayout.Button("Create Map Data"))
        {

            TextAsset data = new TextAsset("\nMap Name: " + MapName + "\nMap Author: " + MapAuthor);
            AssetDatabase.CreateAsset(data, "Assets/map.txt");
            Debug.Log(AssetDatabase.GetAssetPath(data));
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