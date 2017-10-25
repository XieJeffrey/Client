using UnityEngine;
using System.Collections;
using System.IO;

public class DemoRuntimeEditTable : MonoBehaviour {

    OOFormArray mForm = null;
    string mSavePath;
	// Use this for initialization
	void Start () 
    {
        //Here i save form in "Assets" folder for test, you can change it where you like.
        mSavePath = Application.dataPath + "/RuntimeTable.txt";

        if (File.Exists(mSavePath))
        {
            mForm = OOFormArray.ReadFromFile(mSavePath);
        }
        else
        {
            mForm = OOFormArray.ReadFromResources("OOForm/Tables/RuntimeTable");
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mForm != null)
            {
                //random data
                int width = Random.Range(50, 100);
                Rect rect = new Rect(Input.mousePosition.x - width / 2, Screen.height - (Input.mousePosition.y + width / 2), width, width);
                string name = Random.Range(100, 999).ToString();

                //insert and set data
                mForm.InsertRow(mForm.mRowCount);
                mForm.SetRect(rect, "rect", mForm.mRowCount - 1);
                mForm.SetString(name, "name", mForm.mRowCount - 1);

                //save
                Save();
            }
        }
	}

    void Save()
    {
        mForm.SaveFormFile(mSavePath);
    }

    void OnGUI()
    {
        Rect tmp_rect;
        if(mForm != null)
        {
            for (int i = 1; i < mForm.mRowCount; i++)
            {
                tmp_rect = mForm.GetRect("rect", i);

                if (GUI.Button(tmp_rect, mForm.GetString("name", i)))
                {
                    mForm.DeleteRow(i);
                    Save();
                    return;
                }
            }


        }
    }

    
}
