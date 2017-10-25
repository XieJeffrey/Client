using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class DemoGameConfig : MonoBehaviour {
	
	string mSavePath;
	OOFormArray mForm;
	bool mIsTick;
	Rect mWindowRect;
	
	 
	// Use this for initialization
	void Start () 
	{
		//Here i save form in "Assets" folder for test, you can change it where you like.
		mSavePath = Application.dataPath + "/GameConfig.txt";
		
		if(File.Exists(mSavePath))
		{
			mForm = OOFormArray.ReadFromFile(mSavePath);
		}
		else
		{
			mForm = OOFormArray.ReadFromResources("OOForm/Tables/GameConfig");
		}
		
		//read data by column string name
		int run_times = mForm.GetInt("Value", "RUN_TIMES") + 1;
		mForm.SetInt(run_times,"Value", "RUN_TIMES");
		Save ();
		
		//Read data by enum
		mIsTick = mForm.GetBool("Value", "IS_TICK");
		
		mWindowRect = mForm.GetRect("Value", "WINDOW_RECT");
		
		Debug.Log(mWindowRect.ToString());
	}
	
	void Save()
	{
		//set data by string name
		mForm.SaveFormFile(mSavePath);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 50, 100, 50), "Save"))
		{
			//Set data by enum
			mForm.SetString(DateTime.Now.ToString(), "Value", "SAVE_TIME");
			mForm.SetBool(mIsTick, "Value", "IS_TICK");
			mForm.SetRect(mWindowRect, "Value", "WINDOW_RECT");
			Save();
		}
		
		GUI.Label(new Rect(10, 150, 100, 20), "RunTimes:");
		GUI.Label(new Rect(10, 180, 100, 20), "SaveTime:");
		GUI.Label(new Rect(10, 210, 100, 20), "Text1:");
		GUI.Label(new Rect(10, 240, 100, 20), "Text2:");
		GUI.Label(new Rect(10, 270, 100, 20), "IsTick:");
		
		GUI.Label(new Rect(110, 150, 100, 20), mForm.GetString("Value", "RUN_TIMES"));
		GUI.Label(new Rect(110, 180, 300, 20), mForm.GetString("Value", "SAVE_TIME"));
		
		//Read data and set data by id
		mForm.mData[1][3] = GUI.TextField(new Rect(110, 210, 300, 20), mForm.mData[1][3]);
		mForm.mData[1][4] = GUI.TextField(new Rect(110, 240, 300, 20), mForm.mData[1][4]);
		
		mIsTick = GUI.Toggle(new Rect(110, 270, 100, 20), mIsTick, "");
		
		mWindowRect = GUI.Window (0, mWindowRect, DoMyWindow, "Drag me and save");
	}
	
	void DoMyWindow(int windowID)
	{
		GUI.DragWindow(new Rect(0, 0, 200, 50));
		GUI.Label(new Rect(10, 20, 100, 20), "X:" + mWindowRect.x.ToString() + "  Y:" + mWindowRect.y.ToString());
	}
}
