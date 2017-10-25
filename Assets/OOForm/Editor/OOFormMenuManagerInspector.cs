using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(OOFormMenuManager))]
public class OOFormMenuManagerInspector : Editor
{

    OOFormMenuManager mMenuTarget = null;
    string tmp_string = "";

    public override void OnInspectorGUI()
    {

        if (mMenuTarget == null)
            mMenuTarget = (OOFormMenuManager)target;


        if (GUILayout.Button("Apply Menu"))
        {
            ApplyMenu();
        }

        GUILayout.Space(10);

        GUILayout.Label("OOForm Managers:");
        for (int i = 0; i < mMenuTarget.mTableManagerList.Count; i++)
        {
            if (mMenuTarget.mTableManagerList[i] == null)
            {
                mMenuTarget.mTableManagerList.RemoveAt(i);
                mMenuTarget.mTableMenuItemList.RemoveAt(i);
                break;
            }

            if (mMenuTarget.mTableManagerList.Count > mMenuTarget.mTableMenuItemList.Count)
            {
                int need = mMenuTarget.mTableMenuItemList.Count - mMenuTarget.mTableManagerList.Count;
                for (int j = 0; j < need; i++)
                {
                    mMenuTarget.mTableMenuItemList.Add("");
                }
            }

            GUILayout.BeginHorizontal();
            mMenuTarget.mTableManagerList[i] = (OOFormManager)EditorGUILayout.ObjectField(mMenuTarget.mTableManagerList[i], typeof(OOFormManager), false);

            tmp_string = mMenuTarget.mTableMenuItemList[i];
            mMenuTarget.mTableMenuItemList[i] = EditorGUILayout.TextField(mMenuTarget.mTableMenuItemList[i]);
            if (tmp_string != mMenuTarget.mTableMenuItemList[i])
            {
                SaveConfig();
            }
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                mMenuTarget.mTableManagerList.RemoveAt(i);
                mMenuTarget.mTableMenuItemList.RemoveAt(i);
                SaveConfig();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(30);
        GUILayout.Label("Tables:");
        for (int i = 0; i < mMenuTarget.mTableList.Count; i++)
        {
            if (mMenuTarget.mTableList[i] == null)
            {
                mMenuTarget.mTableList.RemoveAt(i);
                mMenuTarget.mTableMenuList.RemoveAt(i);
                break;
            }

            if (mMenuTarget.mTableList.Count > mMenuTarget.mTableMenuList.Count)
            {
                int need = mMenuTarget.mTableMenuList.Count - mMenuTarget.mTableList.Count;
                for (int j = 0; j < need; i++)
                {
                    mMenuTarget.mTableMenuList.Add("");
                }
            }

            GUILayout.BeginHorizontal();
            mMenuTarget.mTableList[i] = (TextAsset)EditorGUILayout.ObjectField(mMenuTarget.mTableList[i], typeof(TextAsset), false);

            tmp_string = mMenuTarget.mTableMenuList[i];
            mMenuTarget.mTableMenuList[i] = EditorGUILayout.TextField(mMenuTarget.mTableMenuList[i]);
            if (tmp_string != mMenuTarget.mTableMenuList[i])
            {
                SaveConfig();
            }
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                mMenuTarget.mTableList.RemoveAt(i);
                mMenuTarget.mTableMenuList.RemoveAt(i);
                SaveConfig();
            }
            GUILayout.EndHorizontal();
        }



        
        if (Event.current.type == EventType.DragExited)
        {
            AddObjects(DragAndDrop.objectReferences);

        }
    }

    public bool IsValidDragPayload()
    {
        if (DragAndDrop.objectReferences.GetType() == typeof(OOFormManager))
        {
            return true;
        }
        return false;
    }

    public void AddObjects(object[] objs)
    {
        foreach (object obj in objs)
        {
            AddObject(obj);
        }
    }

    public void AddObject(object obj)
    {
        if (obj.GetType() == typeof(OOFormManager))
        {
            if (!mMenuTarget.mTableManagerList.Contains((OOFormManager)obj))
            {
                mMenuTarget.mTableManagerList.Add((OOFormManager)obj);

                string str = "Window/OOForm/" + ((OOFormManager)obj).name;
                if (mMenuTarget.mTableMenuItemList.Count >= mMenuTarget.mTableManagerList.Count)
                {
                    mMenuTarget.mTableMenuItemList[mMenuTarget.mTableManagerList.Count - 1] = str;
                }
                else
                {
                    mMenuTarget.mTableMenuItemList.Add(str);
                }

                SaveConfig();
            }
        }
        else if (obj.GetType() == typeof(TextAsset))
        {
            if (!mMenuTarget.mTableList.Contains((TextAsset)obj))
            {
                mMenuTarget.mTableList.Add((TextAsset)obj);
                string str = "Window/OOForm/" + ((TextAsset)obj).name;
                if (mMenuTarget.mTableMenuList.Count >= mMenuTarget.mTableList.Count)
                {
                    mMenuTarget.mTableMenuList[mMenuTarget.mTableList.Count - 1] = str;
                }
                else
                {
                    mMenuTarget.mTableMenuList.Add(str);
                }
                SaveConfig();
            }
        }
    }

    public void SaveConfig()
    {
        EditorUtility.SetDirty(mMenuTarget);
    }

    public void ApplyMenu()
    {

        string script_path = OOFormSkin.FindAsset("OOFormMenuApply.cs");
        if (script_path == "")
        {


        }
        else
        {
            string script_start = 
@"//Created by MenuManager ...
using UnityEngine;
using System.Collections;
using UnityEditor;

public class OOFormMenuApply
{

";
            string script_end = "\n}";
            
            string script_menu_item = @"
    [MenuItem(""__MenuName__"")]
    static void __FunctionName__()
    {
        string path = ""__ManagerPath__"";
        OOFormManager manager = AssetDatabase.LoadAssetAtPath(path, typeof(OOFormManager)) as OOFormManager;
        if (manager)
        {
            OOFormManagerPop.InitWithManager(manager);
        }
    }

";

            string script_menu_item_table = @"
    [MenuItem(""__MenuName__"")]
    static void __FunctionName__()
    {
        string path = ""__TablePath__"";
        TextAsset text_asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
        if(text_asset)
        {
            OOFormEditor.OpenWithAsset(text_asset);
        }
    }
";
             
            string script_menu = "";
            for (int i = 0; i < mMenuTarget.mTableManagerList.Count; i++)
            {
                string script_txt = script_menu_item.Replace("__MenuName__", mMenuTarget.mTableMenuItemList[i]);
                script_txt = script_txt.Replace("__FunctionName__", "MenuFunction" + i.ToString());
                script_txt = script_txt.Replace("__ManagerPath__", AssetDatabase.GetAssetPath(mMenuTarget.mTableManagerList[i]));
                script_menu += script_txt;
            }


            for (int i = 0; i < mMenuTarget.mTableList.Count; i++)
            {
                string script_text = script_menu_item_table.Replace("__MenuName__", mMenuTarget.mTableMenuList[i]);
                script_text = script_text.Replace("__FunctionName__", "TableMenuFunction" + i.ToString());
                script_text = script_text.Replace("__TablePath__", AssetDatabase.GetAssetPath(mMenuTarget.mTableList[i]));
                script_menu += script_text;
            }

                OOFormTools.WriteFileText(script_path, script_start + script_menu + script_end);
                AssetDatabase.Refresh();
        }


    }
}