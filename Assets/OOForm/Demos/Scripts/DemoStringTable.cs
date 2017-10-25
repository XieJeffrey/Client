using UnityEngine;
using System.Collections;

public class StringTableManager
{
	public static SystemLanguage mLanguage = Application.systemLanguage;
	public static OOFormArray mForm = null;
	
	
	public static string GetString(object key)
	{
		if(mForm == null)
			mForm = OOFormArray.ReadFromResources("OOForm/Tables/StringTable");
		
		return mForm.GetString(mLanguage.ToString(), key); 
	}
	
}


public class DemoStringTable : MonoBehaviour
{
	public GUIText mGuiText;
	
	
	// Use this for initialization
	void Start () 
	{
		Debug.Log(StringTableManager.GetString(EStringTable.String_Quit));
		Debug.Log(StringTableManager.GetString("String_Attack"));
		Debug.Log(StringTableManager.mForm.GetString("English", "String_Tips"));
		
		SetLanguage(SystemLanguage.English);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void SetLanguage(SystemLanguage language)
	{
		StringTableManager.mLanguage = language;
		mGuiText.text = StringTableManager.GetString(EStringTable.String_Tips);
	}
	
	void OnGUI()
	{
		string english_btn = StringTableManager.mLanguage == SystemLanguage.English ? ("--[" + StringTableManager.GetString("String_English") +"]--"): StringTableManager.GetString("String_English");
		string chinese_btn = StringTableManager.mLanguage == SystemLanguage.Chinese ? ("--[" + StringTableManager.GetString("String_Chinese") +"]--"): StringTableManager.GetString("String_Chinese");
		
		if(GUI.Button(new Rect(10, 50, 100, 50), english_btn))
		{
			SetLanguage(SystemLanguage.English);
		}
		if(GUI.Button(new Rect(110, 50, 100, 50), chinese_btn))
		{
			SetLanguage(SystemLanguage.Chinese);
		}
		
		//1 - Read by id
		GUI.Label(new Rect(10, 150, 200, 20), StringTableManager.GetString(1));
		
		//2 - Read by enum
		GUI.Label(new Rect(10, 180, 200, 20), StringTableManager.GetString(EStringTable.String_Quit));
		
		//3 - Read by key string
		GUI.Label(new Rect(10, 210, 200, 20), StringTableManager.GetString(EStringTable.String_Attack.ToString()));
		GUI.Label(new Rect(10, 240, 200, 20), StringTableManager.GetString("String_Start"));
		
	}
}
