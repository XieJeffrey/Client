using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OOFormMenuManager : ScriptableObject
{
    //Manager list
    public List<OOFormManager> mTableManagerList = new List<OOFormManager>();
    public List<string> mTableMenuItemList = new List<string>();

    //Table list
    public List<TextAsset> mTableList = new List<TextAsset>();
    public List<string> mTableMenuList = new List<string>();
}