﻿using System.Collections;
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

    public static void CheckAppVersion(Action<bool> callBack)
    {
        DoCheckAppVersion(callBack);
    }

    public static IEnumerator DoCheckAppVersion(Action<bool> callBack)
    {
        WWW www = new WWW(AppConst.AppUrl);
        yield return www;
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                byte[] bytes = www.bytes;
                string str = System.Text.Encoding.UTF8.GetString(bytes);
                Util.Log("----------检查app版本->  local:{0}  server:{1}",AppConst.AppVersion,int.Parse(str));
                callBack(AppConst.AppVersion >= int.Parse(str));
            }
        }
    }

    public static void CopyResToLocal(Action callBack)
    {
        Main.Mono.StartCoroutine(DoCopyResToLocal(callBack));
    }

    public static IEnumerator DoCopyResToLocal(Action callBack)
    {
        WaitForEndOfFrame weof = new WaitForEndOfFrame();
        string[] fileInfo = Directory.GetFiles(Application.streamingAssetsPath);
        for (int i = 0; i < fileInfo.Length; i++)
        {
            int lastIndex = fileInfo[i].LastIndexOf("\\") + 1;
            string fileName = fileInfo[i].Substring(lastIndex, fileInfo[i].Length - lastIndex);
            try
            {
                Util.Log("-----文件复制中:{0}", fileName);
                File.Copy(fileInfo[i], Util.DataPath, true);
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.StackTrace);
            }
            yield return weof;
        }
        PlayerPrefs.SetInt("ResVersion", AppConst.ResVersion);
        callBack();
    }

    public static void UpdateBundleRes(Action callBack)
    {
        Main.Mono.StartCoroutine(DoUpdateBundleRes(callBack));
    }

    public static IEnumerator DoUpdateBundleRes(Action callBack)
    {
        Util.Log("------资源对比更新中----");
        Dictionary<string, string> md5Dic = new Dictionary<string, string>();
        Dictionary<string, string> newMd5Dic = new Dictionary<string, string>();
        #region 获取本地资源MD5文件
        string path = Util.DataPath + "/files.txt";
        if (File.Exists(path))
        {

#if UNITY_EDITOR          
            WWW www = new WWW("file:/" + path);
#else		   
		    WWW www = new WWW(url);
#endif
            string[] serverMD5File = new string[] { };
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    byte[] bytes = www.bytes;
                    string str = System.Text.Encoding.UTF8.GetString(bytes);
                    string[] md5List = System.Text.Encoding.UTF8.GetString(bytes).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < md5List.Length; i++)
                    {
                        string[] tmp = md5List[i].Split('|');
                        tmp[1] = tmp[1].Replace("\n", "");
                        md5Dic.Add(tmp[0], tmp[1].Replace("\r", "").Trim());
                    }
                }
            }
            else
            {
                Util.LogError(www.error);
            }
            #endregion

            #region 获取服务器资源MD5文件

            www = new WWW(AppConst.ResUrl);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    string tmpStr = System.Text.Encoding.UTF8.GetString(www.bytes);
                    serverMD5File = tmpStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < serverMD5File.Length; i++)
                    {
                        string[] tmp = serverMD5File[i].Split('|');
                        tmp[1] = tmp[1].Replace("\n", "");
                        newMd5Dic.Add(tmp[0], tmp[1].Replace("\r", "").Trim());
                    }
                }
            }
            #endregion

            #region 文件比对并更新
            List<string> updateFileList = new List<string>();
            foreach (var tmp in newMd5Dic)
            {
                if (md5Dic.ContainsKey(tmp.Key))
                {
                    if (md5Dic[tmp.Key].CompareTo(tmp.Value) != 0)
                    {
                        updateFileList.Add(tmp.Key);
                    }
                }
                else
                    updateFileList.Add(tmp.Key);
            }

            for (int i = 0; i < updateFileList.Count; i++)
            {
                string filePath = Util.DataPath + "/" + updateFileList[i];
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                www = new WWW(AppConst.UpdateUrl + "/" + updateFileList[i]);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    if (www.isDone)
                    {
                        File.WriteAllBytes(filePath, www.bytes);
                        Util.Log("{0} update finish....", filePath);
                    }
                }
            }

            File.WriteAllLines(Util.DataPath + "/version.txt", serverMD5File);
            callBack();
            #endregion

        }
    }
}