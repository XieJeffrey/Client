// ****************************************************************
// 作	 者：蔡泽深
// 创建时间：2019-01-13 04:33:38
// 备	 注：
// 修改内容：										修改者姓名：
// ****************************************************************

using System;
using UnityEngine;
using UnityEditor;

namespace XX {
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(XXButton))]
    public class XXButtonInspector : UnityEditor.UI.ButtonEditor {
        private SerializedProperty isPlayAnima;

        protected override void OnEnable() {
            base.OnEnable();

            isPlayAnima = serializedObject.FindProperty("isPlayAnima");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(isPlayAnima);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
            serializedObject.Update();
           

        }
    }

}
