using UnityEngine;
using System.Collections;

public class DemoNetTable : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(StartGetNetTable());
	}

    OOFormArray mFormArray = null;

    IEnumerator StartGetNetTable()
    {
        string url = @"http://ooform.ooroom.com/NetItem.txt";

        Debug.Log(url);
        WWW www = new WWW(url);

        yield return www;

        System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
        mFormArray = OOFormArray.GetForm(converter.GetString(www.bytes));
    }


	// Update is called once per frame
	void Update () 
    {
	    
	}

    void OnGUI()
    { 
        if(mFormArray != null)
        {
            for (int i = 0; i < mFormArray.mRowCount; i++)
            {
                for (int j = 0; j < mFormArray.mColumnCount; j++)
                {
                    GUI.Label(new Rect(j * 100 + 100, i * 25 + 100, 100, 23), mFormArray.mData[j][i]);
                }
            }
        }
    }
}
