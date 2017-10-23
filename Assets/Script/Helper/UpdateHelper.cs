using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class UpdateHelper  {
    private static int copyIndex = 0;
    private static int copyFileCount = 0;

    public static void CopyResToPersistenDataPath(Action callBack)
    {
        copyIndex = 0;
        copyFileCount = Directory.GetFiles(Util.AssetDirname).Length;
        if (copyFileCount == 0)
        {
            Util.Log("StreamAsset找不到资源");
            Application.Quit();
            return;
        }
    }
}
