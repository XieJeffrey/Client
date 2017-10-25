using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(OOFormManager))]
public class OOFormManagerInspector : Editor 
{
	public OOFormManager mTarget = null;

	public override void OnInspectorGUI ()
	{
		if(mTarget == null)
			mTarget = (OOFormManager)target;

        
		
		if(GUILayout.Button("Open Manager..."))
		{
            OOFormManagerPop.InitWithManager(mTarget);
		}

        GUILayout.Space(10);
		
		for(int i = 0; i < mTarget.mTableList.Count; i++)
		{
			if(mTarget.mTableList[i] == null)
			{
				mTarget.mTableList.RemoveAt(i);
				break;
			}
			
			GUILayout.BeginHorizontal();
			mTarget.mTableList[i] = (TextAsset)EditorGUILayout.ObjectField(mTarget.mTableList[i] , typeof(UnityEngine.TextAsset), false);
			if(GUILayout.Button("Open"))
			{
				//OOFormEditor.InitWithAsset(mTarget.mTableList[i] );
                //OOFormManagerPop.InitWithManager(mTarget, i);
                OOFormEditor.OpenWithAsset(mTarget.mTableList[i]);
			}
            if (OOFormOption.mHasOtherEditor)
            {
                if (GUILayout.Button("Other"))
                {
                    OpenWithExcel(mTarget.mTableList[i]);
                }
            }
            if (GUILayout.Button("X", GUILayout.Width(20)))
			{
				mTarget.mTableList.RemoveAt(i);
			}
			GUILayout.EndHorizontal();
		}
		
		
		if(Event.current.type == EventType.DragExited)
		{
			AddObjects(DragAndDrop.objectReferences);
			
		}
	}

    public void OpenWithExcel(TextAsset asset)
    {
        string path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
        string all_path = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + path;
        all_path = all_path.Replace("/", @"\");

        System.Diagnostics.Process.Start(OOFormOption.mOtherEditorPath, all_path);
    }
     
	public void AddObjects(object[] objs)
	{
		foreach(object obj in objs)
		{
			AddObject(obj);
		}
	}
	
	public void AddObject(object obj)
	{
				if(obj.GetType() == typeof(TextAsset))
				{
					if(!mTarget.mTableList.Contains((TextAsset)obj))
					{
						mTarget.mTableList.Add ((TextAsset)obj);
						SaveConfig();
					}
				}
	}
	
	public void SaveConfig()
    {
        EditorUtility.SetDirty(mTarget);
    }
}


