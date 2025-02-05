using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HatToolsEditorWindow : EditorWindow
{
    Texture2D bannerImage;
    [MenuItem("Get To Work Tools/Hat Tools (WIP)")]
    public static void ShowWindow()
    {
        HatToolsEditorWindow window = GetWindow<HatToolsEditorWindow>();
        window.titleContent = new GUIContent("Get To Work Hat Tools");
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
            GUI.Label(bannerRect, "Hat Tools!", textStyle);
            GUILayout.Space(110);
        }
        else
        {
            GUILayout.Label("(Banner image not found. Place 'banner.png' in Resources.)", EditorStyles.centeredGreyMiniLabel);
        }

        GUILayout.Label("Hat Tools", centered);
    }
}
