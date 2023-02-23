using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XX {
    public class UIEvent {
        public UIEventType eventType;
        public GameObject gameObject;
        public Action<object[]> handle;
    }

    public class EventComponent :MonoBehaviour {
        private List<UIEvent> uiEventList = new List<UIEvent>();

        #region UI消息分发
        public void On(UIEventType m_event, GameObject go, Action<object[]> handle) {
            UIEvent tmp = new UIEvent();
            tmp.eventType = m_event;
            tmp.gameObject = go;
            tmp.handle = handle;
            if (uiEventList.Contains(tmp) == false) {
                uiEventList.Add(tmp);
            }
        }

        public void RemoveEvent(UIEventType m_event) {
            for (int i = 0; i < uiEventList.Count; i++) {
                if (uiEventList[i].eventType == m_event) {
                    uiEventList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveEvent(GameObject go) {
            for (int i = 0; i < uiEventList.Count; i++) {
                if (uiEventList[i].gameObject.GetInstanceID() == go.GetInstanceID()) {
                    uiEventList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Emit(UIEventType m_event, params object[] args) {
            for (int i = 0; i < uiEventList.Count; i++) {
                if (uiEventList[i].eventType == m_event && uiEventList[i].gameObject.activeInHierarchy) {
                    uiEventList[i].handle?.Invoke(args);
                }
            }
        }
    }
    #endregion   
}

