using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

//OOFormEditor
public class OOFormEditor : EditorWindow
{
    //Opening windows
    public static Dictionary<string, OOFormEditor> mOpenWindowDic = new Dictionary<string,OOFormEditor>();
    //Selection text
    public static TextAsset mSelectionText = null;

    int mLeftBarWidth = 150;

    //config OOFormEditor
    public string mAssectPathString = "OOFormEditor";

    public OOFormArray mSelectionTable = new OOFormArray();
    public Vector2 scrollPosition = new Vector2();

    //Opening table data scroll value.
    Vector2 mFormDataScroll = Vector2.zero;
    //Opening table data scroll area value.
    Vector2 mFormDataAreaScroll = Vector2.zero;

    //Opening table infos
	public TextAsset _mAsset;
    public TextAsset mSelectionAsset;
    public string mSelectionAssetPath;

    //String to search
    public string mSearchString = "";
    public Color mOldColor;
	
	public int mCurrentRowPage = 1;
	public int mCurrentColumnPage = 1;
	 
    
	//one page row/column amount
    public int mPageRowCount = 30;
	public int mPageColumnCount = 10;
    
    //colors
    public Color mFormHeadColor = Color.green;
	public Color mFormFirstRow = Color.red;
	public Color mFormColor1 = new Color(0.9f, 0.9f, 0.9f, 1f);
	public Color mFormColor2 = new Color(0.7f, 0.7f, 0.7f, 1f);
	public Color mSearchColor = Color.yellow;
	
	//Is auto backup when save
	public static bool mIsBackup = false;

    bool mIsModify = false;

    public static GUIStyle mStyleTextField;
    //config "OOForm/Form/OOFormEditor"
    [MenuItem("Window/OOForm/OOFormEditor")]
    static void Init()
    {
		UpdateSelection();
        OpenWithAsset(mSelectionText);
    }

    public static void OpenWithAsset(TextAsset textAsset)
    {
        OOFormEditor window = (OOFormEditor)ScriptableObject.CreateInstance(typeof(OOFormEditor));
        window.SetAsset(textAsset);
        window.Show();
    }

    void SetAsset(TextAsset textAsset)
    {
        mSelectionAsset = textAsset;
        if (textAsset == null)
        {
            mSelectionTable = null;
            mSelectionAssetPath = "";
        }
        else
        {
            mSelectionTable = OOFormArray.GetForm(textAsset.text);
            mSelectionAssetPath = AssetDatabase.GetAssetPath(textAsset.GetInstanceID());
        }
    }

    GUIStyle GetTextField()
    {
        if (mStyleTextField == null)
        {
            mStyleTextField = new GUIStyle(GUI.skin.textField);
            mStyleTextField.fontSize = 13;
        }
        return mStyleTextField;
    }

    static void UpdateSelection()
    {
        foreach (object obj in Selection.objects)
        {
            if (obj.GetType() == typeof(TextAsset))
            {
                mSelectionText = (TextAsset)obj;
                break;
            }
        }
    } 

    void ReadFromTextAsset()
    {
        SetAsset(mSelectionAsset);
    }

    void SaveSelectTable()
    {
        if (mSelectionTable != null)
        {
            if (mIsModify)
            {
                mIsModify = false;
                mSelectionTable.SaveFormFile(mSelectionAssetPath);
                AssetDatabase.Refresh();
            }
        }
    }


    void OnGUI()
    {
        if (mSelectionTable == null && mSelectionAsset != null)
        {
            ReadFromTextAsset();
        }
        else if(mSelectionTable != null)
        {
            if (mSelectionTable.mColumnCount <= 0)
            {
                ReadFromTextAsset();
            }
        }

        DrawToolBar();

        mFormDataAreaScroll = GUILayout.BeginScrollView(mFormDataAreaScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        DrawFormData();
        GUILayout.EndScrollView();

        DrawTableToolBar();
    }

    /// <summary>
    /// Get selection table info
    /// </summary>
    /// <returns></returns>
    string GetSelectionAssetInfo()
    {
        if (mSelectionAsset == null)
        {
            return "";
        }
        return mSelectionAsset.name +  " -- Column:" + mSelectionTable.mColumnCount.ToString() + "  Row:" + mSelectionTable.mRowCount.ToString();
    }


    void OnSelectionChange()
    {
        UpdateSelection();
        if (mSelectionAsset == null)
        {
            SetAsset(mSelectionText);
        }
        Repaint();
    }



    void DrawFormData()
    {
        if (mSelectionTable == null)
            return;

        string tmp_string = "";
        Color tmp_color;

        //+ - buttons
        GUILayout.BeginHorizontal(); //3A+


        tmp_color = GUI.backgroundColor;

        if (mIsModify)
        {
            GUI.backgroundColor = Color.blue;
        }
        if (GUILayout.Button("Save", GUILayout.Width(50)))
        {
            SaveSelectTable();
            GUI.FocusControl("ForFouce");
        }
        GUI.backgroundColor = tmp_color;

        for (int i = 0; i < mSelectionTable.mColumnCount; i++)
        {

            if (i < (mCurrentColumnPage - 1) * mPageColumnCount || i > mCurrentColumnPage * mPageColumnCount - 1)
            {
                if (i != 0)
                    continue;
            }
            if (GUILayout.Button("<--"))
            {
                mSelectionTable.MoveColumnLeft(i);
                mIsModify = true;
                GUI.FocusControl("ForFouce");
                break;
            }
            if (GUILayout.Button("-->"))
            {
                mSelectionTable.MoveColumnRight(i);
                mIsModify = true;
                GUI.FocusControl("ForFouce");
                break;
            }
        }
        if (GUILayout.Button(" ", GUILayout.Width(30)))
        {
        }
        GUILayout.EndHorizontal(); //3A-



        //+ - buttons
        GUILayout.BeginHorizontal(); //3A+

        tmp_color = GUI.backgroundColor;

        if (mIsModify)
        {
            GUI.backgroundColor = Color.yellow;
        }
        if (GUILayout.Button("Revert", GUILayout.Width(50)))
        {
            //ShowTableData(mSelectionIndex);
            SetAsset(mSelectionAsset);
            mIsModify = false;
            GUI.FocusControl("ForFouce");
        }
        GUI.backgroundColor = tmp_color;

        for (int i = 0; i < mSelectionTable.mColumnCount; i++)
        {

            if (i < (mCurrentColumnPage - 1) * mPageColumnCount || i > mCurrentColumnPage * mPageColumnCount - 1)
            {
                if (i != 0)
                    continue;
            }
            if (GUILayout.Button("+ " + i.ToString()))
            {
                mSelectionTable.InsertColumn(i);
                GUI.FocusControl("ForFouce");
                mIsModify = true;
                break;
            }
            if (GUILayout.Button("-"))
            {
                mSelectionTable.DeleteColumn(i);
                GUI.FocusControl("ForFouce");
                mIsModify = true;
                break;
            }
        }
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            mSelectionTable.InsertColumn(mSelectionTable.mColumnCount);
            GUI.FocusControl("ForFouce");
            mIsModify = true;
        }
        GUILayout.EndHorizontal(); //3A-

        //first line
        tmp_color = GUI.backgroundColor;
        GUI.backgroundColor = mFormHeadColor;

        GUILayout.BeginHorizontal(); //3B+
        if (GUILayout.Button("", GUILayout.Width(50)))
        {

        }
        for (int i = 0; i < mSelectionTable.mColumnCount; i++)
        {
            if (i < (mCurrentColumnPage - 1) * mPageColumnCount || i > mCurrentColumnPage * mPageColumnCount - 1)
            {
                if (i != 0)
                    continue;
            }
            tmp_string = mSelectionTable.mData[i][0];
            mSelectionTable.mData[i][0] = EditorGUILayout.TextField(mSelectionTable.mData[i][0]);
            if (tmp_string != mSelectionTable.mData[i][0])
                mIsModify = true;

        }
        if (GUILayout.Button("", GUILayout.Width(30)))
        {

        }
        GUILayout.EndHorizontal(); //3B-
        GUI.backgroundColor = tmp_color;

        //all
        mFormDataScroll = GUILayout.BeginScrollView(mFormDataScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));//3C+

        for (int j = 0; j < mSelectionTable.mRowCount; j++)
        {
            if (j < (mCurrentRowPage - 1) * mPageRowCount || j > mCurrentRowPage * mPageRowCount - 1)
            {
                if (j != 0)
                    continue;
            }

            if (j % 2 == 0)
            {
                GUI.backgroundColor = mFormColor1;
            }
            else
            {
                GUI.backgroundColor = mFormColor2;
            }
            if (j == 0)
            {
                GUI.backgroundColor = mFormFirstRow;
            }

            GUILayout.BeginHorizontal(); //3D+
            if (GUILayout.Button(j.ToString(), GUILayout.Width(50), GUILayout.Height(20)))
            {
                mSelectionTable.InsertRow(j);
                mIsModify = true;
            }
            for (int i = 0; i < mSelectionTable.mColumnCount; i++)
            {
                if (i < (mCurrentColumnPage - 1) * mPageColumnCount || i > mCurrentColumnPage * mPageColumnCount - 1)
                {
                    if (i != 0)
                        continue;
                }

                tmp_string = mSelectionTable.mData[i][j];
                Color tmp_color_2 = GUI.backgroundColor;
                if (tmp_string.Contains(mSearchString) && mSearchString != "")
                {
                    GUI.backgroundColor = mSearchColor;
                }

                mSelectionTable.mData[i][j] = EditorGUILayout.TextField(mSelectionTable.mData[i][j], GetTextField(), GUILayout.Height(20));
                if (tmp_string != mSelectionTable.mData[i][j])
                {
                    mIsModify = true;
                }
                GUI.backgroundColor = tmp_color_2;
            }
            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(20)))
            {
                mSelectionTable.DeleteRow(j);
                GUI.FocusControl("ForFouce");
                mIsModify = true;
            }
            GUILayout.EndHorizontal(); //3D-

            GUI.backgroundColor = tmp_color;
        }

        if (GUILayout.Button("+", GUILayout.Width(50), GUILayout.Height(20)))
        {
            mSelectionTable.InsertRow(mSelectionTable.mRowCount);
            GUI.FocusControl("ForFouce");
            mIsModify = true;
        }

        GUILayout.EndScrollView();//3C-
    }

    void DrawToolBar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));   //2A+

        GUILayout.BeginHorizontal(GUILayout.Width(mLeftBarWidth - 6));  //2B+
        GUI.SetNextControlName("ForFouce");

        _mAsset = mSelectionAsset;
        mSelectionAsset = (TextAsset)EditorGUILayout.ObjectField(mSelectionAsset, typeof(TextAsset), false);
        if (_mAsset != mSelectionAsset)
        {
            ReadFromTextAsset();
        }

        GUILayout.EndHorizontal();  //2B-

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));//2C+

        if (mSelectionAsset != null)
        {

            if (GUILayout.Button("Import", EditorStyles.toolbarDropDown, GUILayout.Width(65)))
            {
                GUIContent[] menuItems = new GUIContent[] {
				new GUIContent("from OOForm table..."),
				new GUIContent("from Json..."),
                new GUIContent("from XML..."),
                new GUIContent("from CSV...")
			    };

                EditorUtility.DisplayCustomMenu(new Rect(mLeftBarWidth, 16, 0, 0), menuItems, -1,
                    delegate(object userData, string[] options, int selected)
                    {
                        switch (selected)
                        {
                            case 0:
                                {
                                    string path = EditorUtility.OpenFilePanel("Select file to import!", "", "txt");
                                    if (path != "")
                                    {
                                        mSelectionTable = OOFormArray.ReadFromFile(path);
                                        mIsModify = true;
                                    }
                                }
                                break;
                            case 1: 
                                {
                                    string path = EditorUtility.OpenFilePanel("Select file to import!", "", "txt");
                                    if (path != "")
                                    {
                                        mSelectionTable = OOFormArray.ReadFromJsonFile(path);
                                        mIsModify = true;
                                    }
                                }
                                break;
                            case 2:
                                {
                                    string path = EditorUtility.OpenFilePanel("Select file to import!", "", "xml");
                                    if (path != "")
                                    {
                                        mSelectionTable = OOFormArray.ReadFromXMLFile(path);
                                        mIsModify = true;
                                    }
                                }
                                break;
                            case 3:
                                {
                                    string path = EditorUtility.OpenFilePanel("Select file to import!", "", "csv");
                                    if (path != "")
                                    {
                                        mSelectionTable = OOFormArray.ReadFormCSVFile(path);
                                        mIsModify = true;
                                    }
                                }
                                break;
                        }
                    }
                    , null);
            
            }
            if (GUILayout.Button("Export", EditorStyles.toolbarDropDown, GUILayout.Width(65)))
            {
                GUIContent[] menuItems = new GUIContent[] {
				new GUIContent("to OOForm table..."),
				new GUIContent("to Json..."),
                new GUIContent("to XML..."),
                new GUIContent("to CSV...")
			    };

                EditorUtility.DisplayCustomMenu(new Rect(mLeftBarWidth + 65, 16, 0, 0), menuItems, -1,
                    delegate(object userData, string[] options, int selected)
                    {
                        switch (selected)
                        {
                            case 0:
                                {
                                    string path = EditorUtility.SaveFilePanel("Select file to save!", "", "", "txt");
                                    if (path != "")
                                    {
                                       OOFormTools.WriteFileText(path, mSelectionTable.ToString());
                                    }
                                }
                                break;
                            case 1:
                                {
                                    string path = EditorUtility.SaveFilePanel("Select file to save!", "", "", "txt");
                                    if (path != "")
                                    {
                                        OOFormTools.WriteFileText(path, mSelectionTable.ToJsonString());
                                    }
                                }
                                break;
                            case 2:
                                {
                                    string path = EditorUtility.SaveFilePanel("Select file to save!", "", "", "xml");
                                    if (path != "")
                                    {
                                        OOFormTools.WriteFileText(path, mSelectionTable.ToXMLString(), System.Text.Encoding.UTF8);
                                    }
                                
                                }
                                break;
                            case 3:
                                {
                                    string path = EditorUtility.SaveFilePanel("Select file to save!", "", "", "csv");
                                    if (path != "")
                                    {
                                        OOFormTools.WriteFileText(path, mSelectionTable.ToCSVString(), System.Text.Encoding.UTF8);
                                    }
                                }
                                break;
                        }
                    }
                    , null);
            }

            GUILayout.Label("");

            GUILayout.Label("Search:", GUILayout.Width(50));
            mSearchString = GUILayout.TextField(mSearchString, OOFormSkin.OStyle_ToolbarSearch, GUILayout.Width(120));
            GUILayout.Label("", OOFormSkin.OStyle_ToolbarSearchRightCap);

            GUILayout.Label("    Column Page:", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("<", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                mCurrentColumnPage--;
                mCurrentColumnPage = Mathf.Max(mCurrentColumnPage, 1);
                GUI.FocusControl("ForFouce");
            }
            int max_column_page = mSelectionTable.mColumnCount / mPageColumnCount + 1;
            GUILayout.Label(mCurrentColumnPage.ToString() + "/" + max_column_page.ToString(), EditorStyles.textField, GUILayout.Width(50));

            if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                mCurrentColumnPage++;
                mCurrentColumnPage = Mathf.Min(mCurrentColumnPage, max_column_page);
                GUI.FocusControl("ForFouce");
            }
        }
        GUILayout.EndHorizontal();  //2C-

        GUILayout.EndHorizontal();  //2A-
    }


    void DrawTableToolBar()
    {
        if (mSelectionTable == null)
            return;

        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));   //2A+

        //EditorGUILayout.ObjectField(mSelectionAsset, typeof(TextAsset), false, GUILayout.ExpandWidth(false));

        GUILayout.Label(GetSelectionAssetInfo(), GUILayout.ExpandWidth(true));

        GUILayout.Label("Row Page:", GUILayout.ExpandWidth(false));
        if (GUILayout.Button("^", EditorStyles.toolbarButton, GUILayout.Width(40)))
        {
            mCurrentRowPage--;
            mCurrentRowPage = Mathf.Max(mCurrentRowPage, 1);
            GUI.FocusControl("ForFouce");
        }

        int max_row_page = mSelectionTable.mRowCount / mPageRowCount + 1;
        GUILayout.Label(mCurrentRowPage.ToString() + "/" + max_row_page.ToString(), EditorStyles.textField, GUILayout.Width(50));
        if (GUILayout.Button("v", EditorStyles.toolbarButton, GUILayout.Width(40)))
        {
            mCurrentRowPage++;
            mCurrentRowPage = Mathf.Min(mCurrentRowPage, max_row_page);
            GUI.FocusControl("ForFouce");
        }

        GUILayout.EndHorizontal();  //2A-
    }
}