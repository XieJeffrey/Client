using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XX;
using Proto.Promises;

public class App : MonoBehaviour
{
    public GameObject reporter;
    public static ResourceManager ResourceMgr;
    public static UIComponent UI;
    public static EventComponent Event;
    public static TimerComponent Timer;
    public static DataComponent Data;
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InitComponent().Then(()=> {
            GameInit();
        });
    }

    Promise InitComponent() {
        return Promise.New((deferred) => {
            reporter = GameObject.Find("Reporter");
            reporter.SetActive(AppConst.DebugTool);
            ResourceMgr = ResourceComponent.instance;
            UI = UIComponent.instance;
            Event = EventComponent.instance;
            Timer = GameObject.Find("Components").GetComponent<TimerComponent>();
            Data = GameObject.Find("Components").GetComponent<DataComponent>();
            deferred.Resolve();
        });        
    }

    void GameInit()
    {
        Data.Init();
    }

    void OnInit()
    {
       
    }

    void LoadConfig()
    {
    }


    void OnCheckAppVersion(bool result)
    {
       
    }

    void OnCopyResFinish()
    {
     
    }

    void OnUpdateBundleRes()
    {
        
    }
}
