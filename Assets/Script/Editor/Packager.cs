using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class Packager {
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    /// <summary>
    /// 载入素材
    /// </summary>
    static UnityEngine.Object LoadAsset(string file) {
        if (file.EndsWith(".lua")) file += ".txt";
        return AssetDatabase.LoadMainAssetAtPath("Assets/Builds/" + file);
    }

    [MenuItem("Game/Build iPhone Resource", false, 11)]
    public static void BuildiPhoneResource() { 
        BuildTarget target;
        target = BuildTarget.iOS;
        BuildAssetResource(target, false);
    }

    [MenuItem("Game/Build Android Resource", false, 12)]
    public static void BuildAndroidResource() {
        BuildAssetResource(BuildTarget.Android, true);
    }

    [MenuItem("Game/Build Windows Resource", false, 13)]
    public static void BuildWindowsResource() {
        BuildAssetResource(BuildTarget.StandaloneWindows, true);
    }

    [MenuItem("Game/ CopyConfig")]
    public static void CopyConfig()
    {

        string resPath = Application.dataPath + "/GameRes/Config/tbl";
        string desPath = Application.streamingAssetsPath;
        string[] fileInfos = Directory.GetFiles(resPath);
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Contains("meta"))
                continue;
            string key = fileInfos[i].Replace(resPath + "\\", string.Empty);
            File.Copy(fileInfos[i], desPath + "/"+key, true);
        }
    }


    /// <summary>
    /// 生成绑定素材
    /// </summary>
    public static void BuildAssetResource(BuildTarget target, bool isWin) {
        string dataPath =  Util.DataPath;
        if (Directory.Exists(dataPath)) {
            Directory.Delete(dataPath, true);
        }
        string assetfile = string.Empty;  //素材文件名
        string resPath =  AppDataPath + "/" + Util.AssetDirname + "/";
        if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);
        BuildPipeline.BuildAssetBundles(resPath, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, target);

        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);


        #region 写入bundle的MD5
        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++) {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.Contains(".svn")) continue;

            string md5 = Util.MD5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        #endregion

        #region 写入配置表的MD5
        string configPath = Application.dataPath + "/GameRes/Config/tbl";
        string[] fileInfos = Directory.GetFiles(configPath);
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Contains("meta"))
                continue;
            string md5 = Util.MD5file(fileInfos[i]);
            string key = fileInfos[i].Replace(configPath+"\\",string.Empty);
            sw.WriteLine(key + "|" + md5);
        }
        #endregion


        sw.Close();
        fs.Close();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path) {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names) {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs) {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc) {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }   

    /// <summary>
    /// 目录复制
    /// </summary>
    /// <param name="direcSource">源目录</param>
    /// <param name="direcTarget">目标目录</param>
    static void CopyFolder(string direcSource, string direcTarget)
    {
        if (!Directory.Exists(direcTarget))
            Directory.CreateDirectory(direcTarget);

        DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
        FileInfo[] files = direcInfo.GetFiles();
        foreach (FileInfo file in files)
            file.CopyTo(Path.Combine(direcTarget, file.Name), true);

        DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
        foreach (DirectoryInfo dir in direcInfoArr)
            CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name));

    }
}