using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    public static ResourceManager ResManager;
     void Awake()
    {
        gameObject.AddComponent<OutLog>();
    }


    // Use this for initialization
    void Start () {
        if (AppConst.IsUpdate)
            return;
        else
            GameInit();
	}
	
	

    void GameInit()
    {
        ResManager= gameObject.AddComponent<ResourceManager>();
        ResManager.Initialize(Util.AssetDirname, delegate ()
        {
            Debug.Log("Initialize OK!!!");
            this.OnInit();
        });
       
    }

    void OnInit()
    {
        gameObject.AddComponent<GameManager>();
    }
}
