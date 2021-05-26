using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    [CustomEditor(typeof(AssaultData))]
    public class AssaultDataEditor : Editor {
        List<Editor> _editors;
        private AssaultData _assaultData;

        void OnEnable() {
            _assaultData = target as AssaultData;
            if (_editors == null) {

                _editors = new List<Editor>();
                for (int i = 0; i < _assaultData._teamDatas.Count; i++) {
                    var editor = Editor.CreateEditor(_assaultData._teamDatas[i]);
                    _editors.Add(editor);
                }
            }
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();            

            for (int i = 0; i < _assaultData._teamDatas.Count; i++) {
                EditorGUILayout.BeginVertical();
                _assaultData._foldouts[i] = EditorGUILayout.Foldout(_assaultData._foldouts[i],
                    string.Format("{0} Team", _assaultData._teamTypes[i].ToString()));
                if (_assaultData._foldouts[i])
                    continue;

                EditorGUILayout.BeginHorizontal();
                _assaultData._teamTypes[i] = (TeamType)EditorGUILayout.EnumPopup(
                    "Team Type", _assaultData._teamTypes[i], GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                TeamData teamData = _assaultData._teamDatas[i];
                if (teamData != null) {
                    if (_editors[i] != null) {
                        _editors[i].OnInspectorGUI();
                    }                    
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }

          private void OnDisable() {
            
        }
    }
}
