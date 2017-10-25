using UnityEngine;
using UnityEditor;
using System.Collections;

//OOForm Skin
public class OOFormSkin : MonoBehaviour
{

    public static GUISkin skin_pro = null;
    public static GUISkin skin_free = null;

    static string mPath = "";

    static GUIStyle style_inspectorBG;
    static GUIStyle style_inspectorHeaderBG;
    static GUIStyle style_listBoxBG;
    static GUIStyle style_listBoxItem;
    static GUIStyle style_listBoxSectionHeader;
    static GUIStyle style_bodyBackground;
    static GUIStyle style_dropBox;
    static GUIStyle style_toolbarSearch;
    static GUIStyle style_toolbarSearchClear;
    static GUIStyle style_toolbarSearchRightCap;



    public static string FindAsset(string name)
    {
        string[] files = System.IO.Directory.GetFiles("Assets", name, System.IO.SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                return files[0].Replace('\\', '/');
            else
                return files[0];
        }

        Debug.LogError("OOForm - file " + name + " is missing.");
        return "";
    }

    public static GUISkin LoadSkin()
    {
        GUISkin skin = null;
        if (EditorGUIUtility.isProSkin)
        {
            mPath = FindAsset("OOEditorSkinPro.guiskin");
        }
        else
        {
            mPath = FindAsset("OOEditorSkinFree.guiskin");
        }
        if (mPath != "")
        {
            skin = AssetDatabase.LoadAssetAtPath(mPath, typeof(GUISkin)) as GUISkin;
        }
        return skin;
    }
     
    //static bool mIsProSkin = false;
    static GUISkin mSkin = null;
    public static void Init()
    {

        if (mSkin == null)
        {
            GUISkin skin = GetSkin();
            mSkin = skin;
            style_inspectorBG = skin.FindStyle("OStyle_InspectorBG");
            style_inspectorHeaderBG = skin.FindStyle("OStyle_InspectorHeaderBG");
            style_listBoxBG = skin.FindStyle("OStyle_ListBoxBG");
            style_listBoxItem = skin.FindStyle("OStyle_ListBoxItem");
            style_listBoxSectionHeader = skin.FindStyle("OStyle_ListBoxSectionHeader");
            style_bodyBackground = skin.FindStyle("OStyle_BodyBackground");
            style_dropBox = skin.FindStyle("OStyle_DropBox");
            style_toolbarSearch = skin.FindStyle("OStyle_ToolbarSearch");
            style_toolbarSearchClear = skin.FindStyle("OStyle_ToolbarSearchClear");
            style_toolbarSearchRightCap = skin.FindStyle("OStyle_ToolbarSearchRightCap");
        }
    } 

     
    public static GUISkin GetSkin()
    {
        if (mSkin == null)
        {
            mSkin = LoadSkin();
        }

        return mSkin;
    }

    public static GUIStyle OStyle_InspectorBG { get { Init(); return style_inspectorBG; } }
    public static GUIStyle OStyle_InspectorHeaderBG { get { Init(); return style_inspectorHeaderBG; } }
    public static GUIStyle OStyle_ListBoxBG { get { Init(); return style_listBoxBG; } }
    public static GUIStyle OStyle_ListBoxItem { get { Init(); return style_listBoxItem; } }
    public static GUIStyle OStyle_ListBoxSectionHeader { get { Init(); return style_listBoxSectionHeader; } }
    public static GUIStyle OStyle_BodyBackground { get { Init(); return style_bodyBackground; } }
    public static GUIStyle OStyle_DropBox { get { Init(); return style_dropBox; } }
    public static GUIStyle OStyle_ToolbarSearch { get { Init(); return style_toolbarSearch; } }
    public static GUIStyle OStyle_ToolbarSearchClear { get { Init(); return style_toolbarSearchClear; } }
    public static GUIStyle OStyle_ToolbarSearchRightCap { get { Init(); return style_toolbarSearchRightCap; } }
   
}
