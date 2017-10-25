using UnityEngine;
using System.Collections;

public class DemoOtherFormatTable : MonoBehaviour {

    OOFormArray mFormArray = null;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 75, 150, 30), "Animals(CSV)"))
        {
            TextAsset csv_text = Resources.Load("OOForm/OtherFormatTables/AnimalsCSV.csv") as TextAsset;

            mFormArray = OOFormArray.GetFormByCSVString(csv_text.text);
        }

        if (GUI.Button(new Rect(170, 75, 150, 30), "GameConfig(Json)"))
        {
            TextAsset json_text = Resources.Load("OOForm/OtherFormatTables/GameConfigJson") as TextAsset;

            mFormArray = OOFormArray.GetFormByJsonString(json_text.text);
        }

        if (GUI.Button(new Rect(330, 75, 150, 30), "StringTable(XML)"))
        {
            TextAsset xml_text = Resources.Load("OOForm/OtherFormatTables/StringTableXML") as TextAsset;

            //System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();


            //mFormArray = OOFormArray.GetForm(converter.GetString(www.bytes));
            //byte[] b = "";
            mFormArray = OOFormArray.GetFormByXMLString(xml_text.text);
        }


        if (mFormArray != null)
        {
            for (int i = 0; i < mFormArray.mRowCount; i++)
            {
                for (int j = 0; j < mFormArray.mColumnCount; j++)
                {
                    GUI.Label(new Rect(j * 120 + 100, i * 25 + 150, 100, 23), mFormArray.mData[j][i]);
                }
            }
        }
    }
}
