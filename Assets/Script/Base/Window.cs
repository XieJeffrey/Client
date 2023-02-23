using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Window : MonoBehaviour {
    private bool m_isInit = false;

    #region UI方法

    protected void Rgister(UIEventType UIEventType, Action<object[]> cb) {
        App.Event.On(UIEventType, gameObject, cb);
    }

    public GameObject Find(string goName) {
        return UIHelper.Find(gameObject, goName);
    }

    public GameObject Find(GameObject go, string goName) {
        return UIHelper.Find(go, goName);
    }

    public T Find<T>(GameObject go, string goName) where T : MonoBehaviour {
        return UIHelper.Find<T>(go, goName);
    }

    public void Show(params object[] args) {
        gameObject.SetActive(true);
        OnShow(args);
    }

    public void Hide(params object[] args) {
        try {
            StopAllCoroutines();
            gameObject.SetActive(false);
            OnHide();
        }
        catch (System.Exception e) {
            Debug.LogError(string.Format("{0} hideExecption:{1},{2}", gameObject.name, e.Message, e.StackTrace));
        }
    }

    #endregion

    #region 虚函数  

    public virtual void OnInit() {
        if (!m_isInit) {
            #region 排布子节点的canvas
            int m_sortingOrder = gameObject.GetComponent<Canvas>().sortingOrder;

            Canvas[] m_panel_array = gameObject.GetComponentsInChildren<Canvas>();
            for (int i = 0; i < m_panel_array.Length; i++) {
                if (m_panel_array[i].name == gameObject.name)
                    continue;
                m_panel_array[i].overrideSorting = true;
                m_panel_array[i].sortingOrder = m_panel_array[i].sortingOrder + m_sortingOrder;
            }
            #endregion
        }
    }

    public virtual void OnShow(params object[] args) {

    }

    public virtual void OnHide(params object[] args) {

    }

    public virtual void RegistEvents() {
        Button[] buttonArray = gameObject.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttonArray.Length; i++) {
            UIEventListener.Get(buttonArray[i].gameObject).onClick = OnBtnClick;
        }
    }

    public virtual void OnBtnClick(GameObject go, PointerEventData data) {

    }
    #endregion   
}
