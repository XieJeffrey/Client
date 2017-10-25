//Created by MenuManager ...
using UnityEngine;
using System.Collections;
using UnityEditor;

public class OOFormMenuApply
{


    [MenuItem("Window/OOForm/Demo/DemoTables")]
    static void MenuFunction0()
    {
        string path = "Assets/OOForm/Demos/DemoTables.asset";
        OOFormManager manager = AssetDatabase.LoadAssetAtPath(path, typeof(OOFormManager)) as OOFormManager;
        if (manager)
        {
            OOFormManagerPop.InitWithManager(manager);
        }
    }


}