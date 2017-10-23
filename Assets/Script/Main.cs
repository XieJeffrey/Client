using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static ResourceManager ResManager;
    public static emptyMono Mono;
    void Awake()
    {
        gameObject.AddComponent<OutLog>();
        gameObject.AddComponent<emptyMono>();
    }


    // Use this for initialization
    void Start()
    {
        if (AppConst.IsUpdate)
            return;
        else
            GameInit();
    }


    void GameInit()
    {
        ResManager = gameObject.AddComponent<ResourceManager>();
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


    void OnCheckAppVersion(bool result)
    {
        if (result)
        {
            int localVersion = PlayerPrefs.GetInt("AppResVersion", 0);
            Util.Log("----------检查app版本->  local:{0}  server:{1}", localVersion, AppConst.ResVersion);
            if (localVersion == 0 || AppConst.ResVersion > localVersion)
            {
                //首次安装或者安装包的资源高于本地版本资源的，将安装包的资源解压的本地去
                UpdateHelper.CopyResToLocal(OnCopyResFinish);
            }
            else
            {               
                UpdateHelper.UpdateBundleRes(OnUpdateBundleRes);
            }

        }
        else
        {
            Util.LogError("app版本过低，请更新app");
            Application.Quit();
        }
    }

    void OnCopyResFinish()
    {
        Util.Log("资源解压完毕");
        UpdateHelper.UpdateBundleRes(OnUpdateBundleRes);
    }

    void OnUpdateBundleRes()
    {
        Util.Log("资源更新完成");
        GameInit();
    }

    void StartUpdate()
    {
        if (Util.IsNet == false)
        {
            Util.LogError("没网，更新个JJ");
            GameInit();
            return;
        }
    }
}
