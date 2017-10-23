using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager :MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        WindowFactory.instance.CreateWindow(WindowType.LoginWindow);   
    }

    public void Update()
    {
        WindowManager.instance.OnUpdate();
    }

    public void LateUpdate()
    {
        WindowManager.instance.OnLateUpdate();
    }

}
