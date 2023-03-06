using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XX;
using Proto.Promises;

public class App : MonoBehaviour
{
    private GameObject reporter;
    public static ResourceComponent ResourceMgr;
    public static UIComponent UI;
    public static EventComponent Event;
    public static TimerComponent Timer;
    public static DataComponent Data;
    public static AudioComponent Audio;
    public static GameMgr gameMgr;

    void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        AppStart();

    }
    void AppStart() {
        InitComponent();
        GameInit();
    }

    /// <summary>
    /// 初始化各个部分的组件
    /// </summary>
    /// <returns></returns>
    void InitComponent() {
        reporter = GameObject.Find("Reporter");
        reporter.SetActive(AppConst.DebugTool);
        ResourceMgr = ResourceComponent.instance;
        Timer = GameObject.Find("Components").GetComponent<TimerComponent>();
        Data = GameObject.Find("Components").GetComponent<DataComponent>();
        UI = GameObject.Find("Components").GetComponent<UIComponent>();
        Event = GameObject.Find("Components").GetComponent<EventComponent>();
        Audio=GameObject.Find("Components").GetComponent<AudioComponent>();
        gameMgr = new GameMgr();

    }

    void GameInit() {
        Data.Init().Then(UI.Init);
    }


}
