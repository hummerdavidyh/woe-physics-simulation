using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

   [CustomEditor(typeof(ArrangeManager))]
    public class ArrangeManagerEditor : Editor {

        #region (Variables) Lables
        /// <summary>  </summary>
        GUIContent _totalLabel;
        /// <summary>  </summary>
        GUIContent _rowLabel;
        /// <summary>  </summary>
        GUIContent _spanRowLabel;
        /// <summary>  </summary>
        GUIContent _spanColumnLabel;
        /// <summary>  </summary>
        GUIContent _worldPositionLabl;
        /// <summary>  </summary>
        GUIContent _worldRotateLabel;

        /// <summary>  </summary>
        GUIContent _rebuildLabel;


        /// <summary>  </summary>
        GUIContent _massLabel;
        /// <summary>  </summary>
        GUIContent _dragLabel;
        /// <summary>  </summary>
        GUIContent _centerLabel;
        /// <summary>  </summary>
        GUIContent _sizeLabel;
        /// <summary>  </summary>
        GUIContent _applyLable;

        /// <summary>  </summary>
        GUIContent _applyCustomArrange;
        #endregion

        #region 
        /// <summary>  </summary>
        ArrangeManager _arrangeMgr;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public ArrangeManagerEditor() {
            _totalLabel = new GUIContent("总数");
            _rowLabel = new GUIContent("行数");
            _spanRowLabel = new GUIContent("行间距");
            _spanColumnLabel = new GUIContent("列间距");
            _worldPositionLabl = new GUIContent("世界位置");
            _worldRotateLabel = new GUIContent("世界朝向");

            //_initializeLabel = new GUIContent("初始化阵型");
            _rebuildLabel = new GUIContent("阵列");

            _massLabel = new GUIContent("质量");
            _dragLabel = new GUIContent("阻力");
            _centerLabel = new GUIContent("中心点");
            _sizeLabel = new GUIContent("尺寸");

            _applyLable = new GUIContent("应用");
            _applyCustomArrange = new GUIContent("应用用户自定义阵型");
        }
        #endregion

        #region Unity Methods
        /// <summary>
        /// 
        /// </summary>
        private void OnEnable() {
            if (_arrangeMgr == null) {
                _arrangeMgr = target as ArrangeManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Color oldColor = GUI.color;
            GUI.color = Color.cyan;
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //InitializeBuildEditorWindow();
            GUI.color = oldColor;

            GUILayout.Space(20);

            AssualtsEditorWindow();
        }
        #endregion

        #region (Methods) Windows
        /// <summary>
        /// 
        /// </summary>
        private void AssualtsEditorWindow() {
            GUILayout.BeginVertical();

            for (int assaultIdx = 0; assaultIdx < _arrangeMgr._assaultDatas.Count; assaultIdx++) {
                AssaultData assaultData = _arrangeMgr._assaultDatas[assaultIdx];
                _arrangeMgr._foldouts[assaultIdx] = EditorGUILayout.Foldout(_arrangeMgr._foldouts[assaultIdx],
                     string.Format("{0} 阵列", assaultData.name));
                if (_arrangeMgr._foldouts[assaultIdx])
                    continue;

                EditorGUI.indentLevel += 1;

                for (int teamIdx = 0; teamIdx < assaultData._teamDatas.Count; teamIdx++) {
                    Color oldColor = GUI.color;
                    if (teamIdx == 0) {
                        GUI.color = Color.yellow;
                    }
                    else {
                        GUI.color = Color.green;
                    }

                    TeamData teamData = assaultData._teamDatas[teamIdx];

                    teamData._foldOut = EditorGUILayout.Foldout(teamData._foldOut,
                        string.Format("{0} 阵列的数据区域", teamData.name));
                    if (teamData._foldOut)
                        continue;
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    TotalWindow(teamData);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    RowWindow(teamData);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    SpanRowWindow(teamData);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    SpanColumnWindow(teamData);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    WorldPositionWindow(teamData);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    WorldRotateWindow(teamData);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(_rebuildLabel, GUILayout.Width(120))) {
                        
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    for (int rowIdx = 0; rowIdx < teamData._row; rowIdx++) {
                        teamData._colliders[rowIdx]._foldOut = EditorGUILayout.Foldout(teamData._colliders[rowIdx]._foldOut,
                              string.Format("第{0}行的物理数据", rowIdx + 1));
                        if (teamData._colliders[rowIdx]._foldOut)
                            continue;

                        EditorGUILayout.BeginHorizontal();
                        MassWindow(teamData._rigids[rowIdx], teamIdx, rowIdx);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        DragWindow(teamData._rigids[rowIdx], teamIdx, rowIdx);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        CentreWindow(teamData._colliders[rowIdx], teamIdx, rowIdx);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        SizeWindow(teamData._colliders[rowIdx], teamIdx, rowIdx);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(_applyCustomArrange, GUILayout.Width(180))) {
                        OnApplyCustomArrange(assaultIdx, teamIdx);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    GUI.color = oldColor;
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        /// <param name="index"></param>
        private void TotalWindow(TeamData teamData) {
            teamData._total = EditorGUILayout.DelayedIntField(
                _totalLabel, teamData._total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void RowWindow(TeamData teamData) {
            teamData._row = EditorGUILayout.DelayedIntField(
                _rowLabel, teamData._row, GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void SpanRowWindow(TeamData teamData) {
            teamData._spanRow = EditorGUILayout.DelayedFloatField(
                _spanRowLabel, teamData._spanRow, GUILayout.ExpandWidth(true));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void SpanColumnWindow(TeamData teamData) {
            teamData._spanColumn = EditorGUILayout.DelayedFloatField(
                _spanColumnLabel, teamData._spanColumn, GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void WorldPositionWindow(TeamData teamData) {
            teamData._worldPosition = EditorGUILayout.Vector3Field(
                _worldPositionLabl, teamData._worldPosition, GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void WorldRotateWindow(TeamData teamData) {
            teamData._worldRotation = EditorGUILayout.Vector3Field(
                _worldRotateLabel, teamData._worldRotation, GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phyArgs"></param>
        /// <param name="index"></param>
        private void MassWindow(RigidData rigidData, int teamIdx, int rowIdx) {

            rigidData._mass = EditorGUILayout.DelayedFloatField(
                _massLabel, rigidData._mass, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_applyLable, GUILayout.Width(60))) {
                if (_arrangeMgr) {
                    _arrangeMgr.ApplyCustomMass(teamIdx, rowIdx, rigidData._mass);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phyArgs"></param>
        /// <param name="index"></param>
        private void DragWindow(RigidData rigidData, int teamIdx, int rowIdx) {
            rigidData._drag = EditorGUILayout.DelayedFloatField(
                _dragLabel, rigidData._drag, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_applyLable, GUILayout.Width(60))) {
                _arrangeMgr.ApplyCustomDrag(teamIdx, rowIdx, rigidData._drag);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phyArgs"></param>
        /// <param name="index"></param>
        private void CentreWindow(ColliderData colliderData, int teamIdx, int rowIdx) {
            colliderData._centre =
                EditorGUILayout.Vector3Field(_centerLabel, colliderData._centre);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_applyLable, GUILayout.Width(60))) {
                _arrangeMgr.ApplyCustomColliderCenter(teamIdx, rowIdx, colliderData._centre);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phyArgs"></param>
        /// <param name="index"></param>
        private void SizeWindow(ColliderData colliderData, int teamIdx, int rowIdx) {
            colliderData._size =
                EditorGUILayout.Vector3Field(_sizeLabel, colliderData._size);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_applyLable, GUILayout.Width(60))) {
                _arrangeMgr.ApplyCustomColliderSize(teamIdx, rowIdx, colliderData._size);
            }
        }
        #endregion

        #region Operations
        /// <summary>
        /// 
        /// </summary>
        /// <param name="curAssaultIndex"></param>
        private void OnInitializeBuild(int curAssaultIndex) {
            if (_arrangeMgr != null) {
                _arrangeMgr.InitializeBuild(curAssaultIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curAssaultIndex"></param>
        /// <param name="teamIndex"></param>
        private void OnApplyCustomArrange(int curAssaultIndex, int teamIndex) {
            if (_arrangeMgr != null) {
                if (teamIndex == 0)
                    _arrangeMgr.ApplyArrangeYellowTeam(curAssaultIndex, teamIndex);
                else if(teamIndex == 1)
                    _arrangeMgr.ApplyArrangeGreenTeam(curAssaultIndex, teamIndex);
            }
        }
        #endregion
    }
}
