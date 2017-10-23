using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConst
{
    public const bool IsUpdate = false;
    public const string AppName = "ClientFrame";
    public const int ResVersion = 20171023;
    public const int AppVersion = 20171023;//服务器版本
    public const string ExtName = ".assetbundle";

    public const string ResUrl = "";
    public const string AppUrl = "";


#if ANDROID || UNITY_EDITOR
    public static string UpdateUrl = "http://localhost/Update/Android/";
#else
    public static string UpdateUrl = "http://localhost/Update/IOS/";
#endif
}
