using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using XX;

public class Util : Singleton<Util>
{
    public static bool IsNet
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    public static void Vibrate()
    {
        Handheld.Vibrate();
    } 

    public static string MD5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] resVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < resVal.Length; i++)
            {
                sb.Append(resVal[i].ToString("X2"));
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("MD5file() file,error:{0}" + ex.StackTrace);
        }
    }

    /// <summary>
    /// 获取当天0点的时间戳
    /// </summary>
    /// <returns></returns>
    public static string GetTodayTimeStamp() {
        DateTime now = DateTime.Now.Date.ToLocalTime();
        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

        TimeSpan ts = now - start;
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public static long Today {
        get {
            return long.Parse(GetTodayTimeStamp());
        }
    }

    public static long Yestoday {
        get {
            return Today - 3600 * 24;
        }
    }

    public static long Tomorrow {
        get {
            DateTime now = DateTime.Now.Date.ToLocalTime();
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

            TimeSpan ts = now - start;
            return Convert.ToInt64(ts.TotalSeconds + 3600 * 24);
        }
    }

    public static long Now {
        get {
            DateTime now = DateTime.Now.ToLocalTime();
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

            TimeSpan ts = now - start;
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }

    public static long GetTomorrowLeftTime() {
        DateTime tomorrow = DateTime.Now.Date.AddDays(1).ToLocalTime();
        DateTime now = DateTime.Now.ToLocalTime();

        TimeSpan ts = tomorrow - now;
        return Convert.ToInt64(ts.TotalSeconds);
    }

    public static string sec2hhmmss(long time) {
        long hour = time / 3600;
        long sec = time % 60;
        long min = (time % 3600 - sec) / 60;

        if (hour > 0) {
            return string.Format("{0}:{1}:{2}", hour.ToString().PadLeft(2, '0'), min.ToString().PadLeft(2, '0'), sec.ToString().PadLeft(2, '0'));
        }
        else {
            return string.Format("{0}:{1}", min.ToString().PadLeft(2, '0'), sec.ToString().PadLeft(2, '0'));
        }
    }
}
