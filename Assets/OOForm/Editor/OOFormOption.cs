using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

//Options window
public class OOFormOption : EditorWindow
{
    public static OOFormArray mOptionData = null;

    [MenuItem("Window/OOForm/Options")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(OOFormOption));
    }

    public static bool mHasOtherEditor
    {
        get
        {
            int ret = PlayerPrefs.GetInt("__has_other_editor", 0);
            return ret == 0 ? false : true;
        }
        set
        {
            int set = value ? 1 : 0;
            PlayerPrefs.SetInt("__has_other_editor", set);
            PlayerPrefs.Save();
        }
    }

    public static string mOtherEditorPath
    {
        get
        {
            return PlayerPrefs.GetString("__other_editor_path", "");
        }
        set
        {
            PlayerPrefs.SetString("__other_editor_path", value);
            PlayerPrefs.Save();
        }
    }

    void OnGUI()
    {
        mHasOtherEditor = GUILayout.Toggle(mHasOtherEditor, "Use other editor?(I am sorry that this just support Windows now.)");

        if (mHasOtherEditor)
        {
            GUILayout.Label("Other editor:");
            if (GUILayout.Button(mOtherEditorPath))
            {
                string path = EditorUtility.OpenFilePanel("Select other editor!", "", "*");
                mOtherEditorPath = path;
            }
        }

    }
}
