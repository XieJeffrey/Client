using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XX;


public class UIHelper : Singleton<UIHelper> {
    public static GameObject Find(GameObject m_go, string goName) {
        if (m_go == null)
            return null;

        if (string.IsNullOrEmpty(goName))
            return null;

        Transform trans = m_go.transform.Find(goName);
        if (trans != null)
            return trans.gameObject;

        return null;
    }


    public static T Find<T>(GameObject m_go, string goName) where T : class {
        GameObject go = Find(m_go, goName);

        if (null != go) {
            T component = go.GetComponent<T>();
            return component;
        }

        return null;
    }
}


