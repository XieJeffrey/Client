using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class OOFormCreator : EditorWindow 
{		
	[MenuItem("Assets/Create/OOForm/Simple Table")]
    static void CreateTable()
    {
        string str = GetSelectionFolder();

        int i = 0;
        while (File.Exists(str  + "Table_" + i.ToString() + ".txt"))
        {
            i++;
        }
        FileStream fs;
        fs = File.Create(str  + "Table_" + i.ToString() + ".txt");
        fs.Close();
        AssetDatabase.Refresh();
    }
	
	[MenuItem("Assets/Create/OOForm/OOForm Menu Manager")]
    static void CreateOOFormMenuManager()
    {
		string str = GetSelectionFolder();
		
		int i = 0;
        while (File.Exists(str  + "OOFormMenuManager_" + i.ToString() + ".asset"))
        { 
            i++;
        }
		 
		string file_name = str  + "OOFormMenuManager_" + i.ToString() + ".asset";
		
		OOFormMenuManager manager = ScriptableObject.CreateInstance<OOFormMenuManager>();
		
		AssetDatabase.CreateAsset(manager, file_name);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/OOForm/OOForm Manager")]
    static void CreateOOFormManager()
    {
        string str = GetSelectionFolder();

        int i = 0;
        while (File.Exists(str + "OOFormManager_" + i.ToString() + ".asset"))
        {
            i++;
        }

        string file_name = str + "OOFormManager_" + i.ToString() + ".asset";

        OOFormManager manager = ScriptableObject.CreateInstance<OOFormManager>();

        AssetDatabase.CreateAsset(manager, file_name);
        AssetDatabase.Refresh();
    }
	
	static public string GetSelectionFolder ()
	{
		if (Selection.activeObject != null)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

			if (!string.IsNullOrEmpty(path))
			{
				int dot = path.LastIndexOf('.');
				int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
				if (slash > 0) return (dot > slash) ? path.Substring(0, slash + 1) : path + "/";
			}
		}
		return "Assets/";
	}
}