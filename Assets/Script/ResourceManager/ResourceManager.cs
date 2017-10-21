using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject = UnityEngine.Object;


public class AssetBundleInfo
{
    public AssetBundle m_AssetBundle;
    public int m_ReferenceCount = 0;

    public AssetBundleInfo(AssetBundle assetBundle)
    {
        m_AssetBundle = assetBundle;
        m_ReferenceCount = 0;
    }
}

public class ResourceManager : Singleton<ResourceManager>
{

    class LoadAssetRequest
    {
        public Type assetType;
        public string[] assetNames;
        public Action<UObject[]> sharpFunc;
    }

    string m_BaseDownloadUrl = "";
    string[] m_AllManifest = null;

    AssetBundleManifest m_AssetBundleManifest = null;
    Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
    Dictionary<string, AssetBundleInfo> m_LoadedAssetbundles = new Dictionary<string, AssetBundleInfo>();
    Dictionary<string, List<LoadAssetRequest>> m_loaddRequests = new Dictionary<string, List<LoadAssetRequest>>();
    Dictionary<string, int> m_loadingRequests = new Dictionary<string, int>();

    public void Initialize(string manifestName, Action initOK)
    {
        m_BaseDownloadUrl = "";//todo

    }


    public void LoadPrefab(string abName, string assetName, Action<UObject[]> func)
    {

    }

    public void LoadImage(string abName, string assetName, Action<UObject[]> func)
    {

    }

    public void LoadAudio(string abName, string assetName, Action<UObject> func)
    { }


    string GetRealAssetPath(string abName)
    {
        return "";
    }
}
