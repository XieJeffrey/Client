//This demo show you how to use GetObject<T>() of OOFormArray
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Animal class
class Animal
{
	public string mName = "";
	public int mAttack;
	public int mHP;
	public float mMoveSpeed;
    public string mSkillList
    {
        set
        {
            mSkills = value.Split(',');
        }
    }

	public string[] mSkills;
}

 
public class DemoAnimals : MonoBehaviour 
{
	int mCurrentAnimal = 1;
	Animal mAnimal = null;
    OOFormArray mAnimalArray = null;
    
    void SetAnimal(int animalID)
	{
        mAnimal = mAnimalArray.GetObject<Animal>(animalID);
	}
	
	// Use this for initialization
	void Start () 
	{
        mAnimalArray = OOFormArray.ReadFromResources("OOForm/Tables/Animals");
        SetAnimal(mCurrentAnimal);
	}
	


	// Update is called once per frame
	void Update () 
	{
		
	}

    void PrePage()
    {
        mCurrentAnimal--;
        if (mCurrentAnimal < 1)
        {
            mCurrentAnimal = mAnimalArray.mRowCount - 1;
        }
        SetAnimal(mCurrentAnimal);
    }

    void NextPage()
    {
        mCurrentAnimal++;
        if (mCurrentAnimal >= mAnimalArray.mRowCount)
        {
            mCurrentAnimal = 1;
        }
        SetAnimal(mCurrentAnimal);
    }

	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 50, 100, 50), "<"))
		{
            PrePage();
		}
		GUI.Button(new Rect(110, 50, 50, 50), mCurrentAnimal.ToString());
		if(GUI.Button(new Rect(160, 50, 100, 50), ">"))
		{
            NextPage();
		}
		
		
		GUI.Label(new Rect(10, 150, 100, 20), "Name:");
		GUI.Label(new Rect(10, 180, 100, 20), "Attack:");
		GUI.Label(new Rect(10, 210, 100, 20), "HP:");
		GUI.Label(new Rect(10, 240, 100, 20), "MoveSpeed:");
		GUI.Label(new Rect(10, 270, 100, 20), "Skills:");
		
		if(mAnimal != null)
		{
			GUI.Button(new Rect(110, 150, 100, 20), mAnimal.mName);
			GUI.Button(new Rect(110, 180, 100, 20), mAnimal.mAttack.ToString());
			GUI.Button(new Rect(110, 210, 100, 20), mAnimal.mHP.ToString());
			GUI.Button(new Rect(110, 240, 100, 20), mAnimal.mMoveSpeed.ToString());
			GUI.SelectionGrid(new Rect(110, 270, mAnimal.mSkills.Length * 50, 20), 0, mAnimal.mSkills, mAnimal.mSkills.Length);
		}		
		
	}
}
