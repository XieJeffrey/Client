using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Proto.Promises;
using System;

namespace XX
{
    class UIConfig
    {
        private JArray config;
        public  Dictionary<string, int> uiOrder = new Dictionary<string, int>();
        public List<UIType> preloadList = new List<UIType>();
        public Promise Init() {
            return Promise.New((deferred) => {
                App.ResourceMgr.LoadAssetPro<TextAsset>("Config/UI/ui.json").Then((TextAsset textAsset) => {
                    try {
                        //加载UI配置
                        Debug.Log(textAsset.text);
                        config = JArray.Parse(textAsset.text);
                        //缓存UI层级
                        for (int i = 0; i < config.Count; i++) {
                            string name = config[i]["name"].ToString();
                            if (!uiOrder.ContainsKey(name)) {
                                uiOrder.Add(name, int.Parse(config[i]["order"].ToString()));
                            }
                        }
                        //缓存预加载配置
                        for (int i = 0; i < config.Count; i++) {
                            string name = config[i]["name"].ToString();
                            UIType type = (UIType)Enum.Parse(typeof(UIType), name);
                            bool isPreload = int.Parse(config[i]["isPreload"].ToString())==1;
                            if (!preloadList.Contains(type))
                                preloadList.Add(type);
                        }
                        deferred.Resolve();
                    }
                    catch (Exception e) {
                        deferred.Reject(e);
                        Debug.LogError("===>XX:load ui config fail");
                    }
                });
            });
        }

        public string GetUIAsset(UIType type) {
            string name = type.ToString();
            for (int i = 0; i < config.Count; i++) {
                if (name == config[i]["name"].ToString()) {
                    return config[i]["asset"].ToString();
                }
            }

            return "";
        }

        public int GetUISortingOrder(UIType type) {
            string name = type.ToString();
            for (int i = 0; i < config.Count; i++) {
                if (name == config[i]["name"].ToString()) {
                    return (int.Parse(config[i]["order"].ToString()));
                }
            }

            return 0;
        }
    }

    public class UIComponent : MonoBehaviour
    {
        private UIConfig uiConfig=new UIConfig();//ui配置
        public GameObject UIRoot;//ui根节点
        private Dictionary<UIType, Window> m_winDic = new Dictionary<UIType, Window>();
        private List<UIType> m_lodingwindow = new List<UIType>();

        public Promise Init() {            
            return Promise.New((defeered) => {
                uiConfig.Init().Then(() => {
                    UIRoot = GameObject.Find("GUICamera/Canvas");
                    PreLoad(uiConfig.preloadList).Then(() => {
                        ShowWindow(UIType.start);
                        defeered.Resolve();
                    });
                });
            });
         
        }

        /// <summary>
        /// 预加载窗体
        /// </summary>
        /// <param name="winList"></param>
        /// <returns></returns>
        private Promise PreLoad(List<UIType> winList) {
            return Promise.New((deferred => {
                if (winList.Count == 0) {
                    deferred.Resolve();
                    return;
                }

                List<UIType> loadedWinList = new List<UIType>();
                for (int i = 0; i < winList.Count; i++) {
                    UIType type = winList[i];
                    string asset = uiConfig.GetUIAsset(type);
                    if (string.IsNullOrEmpty(asset)) {
                        deferred.Reject(new Exception(string.Format("wintype:{ 0 },assetName is null")));
                    }
                    App.ResourceMgr.LoadAssetPro<GameObject>(asset).Then((GameObject wingo) => {
                        loadedWinList.Add(type);
                        GameObject go = GameObject.Instantiate(wingo);
                        m_winDic.Add(type, go.GetComponent<Window>());
                        InitWindow(type, go);
                        m_winDic[type].OnInit();
                        m_winDic[type].RegistEvents();
                        if (loadedWinList.Count == winList.Count) {
                            deferred.Resolve();
                        }
                    });
                }
            }));
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="wintype"></param>
        /// <param name="args"></param>
        public void ShowWindow(UIType wintype, params object[] args) {
            if (m_winDic.ContainsKey(wintype)) {
                m_winDic[wintype].Show();
            }
            else {
                CreateWindow(wintype, args);
            }
        }

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="wintype"></param>
        /// <param name="args"></param>
        private void CreateWindow(UIType wintype, params object[] args) {
            if (m_lodingwindow.Contains(wintype)) {
                return;
            }
            m_lodingwindow.Add(wintype);
            string asset = uiConfig.GetUIAsset(wintype);
            if (string.IsNullOrEmpty(asset)) {
                Debug.LogError(string.Format("wintype:{0},assetName is null"));
                return;
            }
            App.ResourceMgr.LoadAssetPro<GameObject>(asset).Then((GameObject winGo) => {
                GameObject go = GameObject.Instantiate(winGo);
                m_winDic.Add(wintype, go.GetComponent<Window>());
                m_lodingwindow.Remove(wintype);
                InitWindow(wintype,go);
                m_winDic[wintype].OnInit();
                m_winDic[wintype].RegistEvents();
                m_winDic[wintype].Show(args);
            });
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        /// <param name="wintype"></param>
        /// <param name="args"></param>
        public void HideWindow(UIType wintype, params object[] args) {
            if (m_winDic.ContainsKey(wintype)) {
                m_winDic[wintype].Hide(args);
            }
        }

        /// <summary>
        /// 初始化窗口到场景里
        /// </summary>
        /// <param name="type"></param>
        /// <param name="go"></param>
        private void InitWindow(UIType type, GameObject go) {
            go.transform.SetParent(UIRoot.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            (go.transform as RectTransform).sizeDelta = (UIRoot.transform as RectTransform).sizeDelta;

            if (go.GetComponent<GraphicRaycaster>() == null)
                go.AddComponent<GraphicRaycaster>();

            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas == null)
                canvas = go.AddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = uiConfig.GetUISortingOrder(type);
        }

        /// <summary>
        /// 判断窗体是否在场景中显示(包括正在创建中的窗体)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsShow(UIType type) {
            if (IsActive(type))
                return true;
            else {

                for (int i = 0; i < m_lodingwindow.Count; i++) {
                    if (m_lodingwindow.Contains(type))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 判断窗体是否在场景中显示
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsActive(UIType type) {
            if (!m_winDic.ContainsKey(type))
                return false;

            return m_winDic[type].gameObject.activeSelf;
        }
    }
}
