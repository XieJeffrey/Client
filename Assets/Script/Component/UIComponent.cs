using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Proto.Promises;
using System;

namespace XX {
    class UIConfig {
        private static JArray config;
        public static List<WindowType> preLoad;
        public static Promise Init() {
            return Promise.New((deferred) => {
                App.ResourceMgr.LoadAssetPro<TextAsset>("config/ui.json").Then((TextAsset textAsset) => {
                    try {
                        Debug.Log(textAsset.text);
                        config = JArray.Parse(textAsset.text);
                        deferred.Resolve();

                        preLoad = new List<WindowType>();
                        for (int i = 0; i < config.Count; i++) {
                            if (((int)config[i]["isPreload"]) == 1) {
                                preLoad.Add((WindowType)((int)config[i]["id"]));
                            }
                        }
                    }
                    catch (Exception e) {
                        deferred.Reject(e);
                        Debug.LogError("===>XX:load ui config fail");
                    }
                });
            });
        }

        public static string GetUIAsset(WindowType type) {
            int id = (int)type;
            for (int i = 0; i < config.Count; i++) {
                if (id == ((int)config[i]["id"])) {
                    return config[i]["asset"].ToString();
                }
            }

            return "";
        }

        public static int GetUISortingOrder(WindowType type) {
            int id = (int)type;
            for (int i = 0; i < config.Count; i++) {
                if (id == ((int)config[i]["id"])) {
                    return ((int)config[i]["order"]);
                }
            }

            return 0;
        }
    }

    public class UIComponent : Singleton<UIComponent>, IProcedure {
        public GameObject UIRoot;
        private Dictionary<WindowType, Window> m_winDic = new Dictionary<WindowType, Window>();
        private List<WindowType> m_lodingwindow = new List<WindowType>();

        public void Init() {
            UIConfig.Init().Then(() => {
                UIRoot = GameObject.Find("GUICamera/Canvas");
                PreLoad(UIConfig.preLoad).Then(() => {
                    InitFinish();
                });
            });
        }

        public void UpdateProgress(float value) { 
        
        }
        public void InitFinish() { }

        /// <summary>
        /// 预加载窗体
        /// </summary>
        /// <param name="winList"></param>
        /// <returns></returns>
        private Promise PreLoad(List<WindowType> winList) {
            return Promise.New((deferred => {
                if (winList.Count == 0) {
                    deferred.Resolve();
                    return;
                }

                List<WindowType> loadedWinList = new List<WindowType>();
                for (int i = 0; i < winList.Count; i++) {
                    WindowType type = winList[i];
                    string asset = UIConfig.GetUIAsset(type);
                    if (string.IsNullOrEmpty(asset)) {
                        deferred.Reject(new Exception(string.Format("wintype:{ 0 },assetName is null")));
                    }
                    App.ResourceMgr.LoadAssetPro<GameObject>(asset).Then((GameObject wingo) => {
                        loadedWinList.Add(type);
                        m_winDic.Add(type, wingo.GetComponent<Window>());
                        InitWindow(type, wingo);
                        m_winDic[type].OnInit();
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
        public void ShowWindow(WindowType wintype, params object[] args) {
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
        private void CreateWindow(WindowType wintype, params object[] args) {
            if (m_lodingwindow.Contains(wintype)) {
                return;
            }
            m_lodingwindow.Add(wintype);
            string asset = UIConfig.GetUIAsset(wintype);
            if (string.IsNullOrEmpty(asset)) {
                Debug.LogError(string.Format("wintype:{0},assetName is null"));
                return;
            }
            App.ResourceMgr.LoadAssetPro<GameObject>(asset).Then((GameObject winGo) => {
                m_winDic.Add(wintype, winGo.GetComponent<Window>());
                m_lodingwindow.Remove(wintype);
                InitWindow(wintype, winGo);
                m_winDic[wintype].OnInit();
                m_winDic[wintype].Show(args);
            });
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        /// <param name="wintype"></param>
        /// <param name="args"></param>
        public void HideWindow(WindowType wintype, params object[] args) {
            if (m_winDic.ContainsKey(wintype)) {
                m_winDic[wintype].Hide(args);
            }
        }

        /// <summary>
        /// 初始化窗口到场景里
        /// </summary>
        /// <param name="type"></param>
        /// <param name="go"></param>
        private void InitWindow(WindowType type, GameObject go) {
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
            canvas.sortingOrder = UIConfig.GetUISortingOrder(type);
        }

        /// <summary>
        /// 判断窗体是否在场景中显示(包括正在创建中的窗体)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsShow(WindowType type) {
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
        public bool IsActive(WindowType type) {
            if (!m_winDic.ContainsKey(type))
                return false;

            return m_winDic[type].gameObject.activeSelf;
        }
    }
}
