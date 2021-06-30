using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class ExplodeArrangeManager : MonoBehaviourSingleton<ExplodeArrangeManager> {
        #region (Variables) Datas
        /// <summary>  </summary>        
        public int _activeExplodeDataIndex;

        /// <summary>  </summary>
        public List<ExplodeData> _explodeDatas = new List<ExplodeData>();
        /// <summary>  </summary>
        [HideInInspector()]
        public List<bool> _foldouts = new List<bool>();
        #endregion
        
        #region (Varibles) Prefabs
        /// <summary>  </summary>
        public GameObject _greenTeamPrefab;
        #endregion

        #region (Varibles) Attck
        [Range(1, 100)]
        public float _attackSpeed;
        #endregion

        #region (Varibles) Teams
        /// <summary> </summary>
        private GameObject _greenTeamRoot;
        /// <summary> </summary>
        private List<Transform> _greenTeams = new List<Transform>();
        #endregion

        #region (varibles)
        private float _scaleW = 1.0f; //宽度缩放比
        private float _scaleH = 1.0f; //高度缩放比
        #endregion

        #region (Unity Methods) 
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        protected override void SingletonStarted() {
            base.SingletonStarted();

            string[] files = Directory.GetFiles(ExplodeData.ASSET_PATH);
            foreach (var file in files) {
                string fileName = AssetUtils.GetFileName(file);
                if (fileName.Contains(".meta")) {
                    continue;
                }

                fileName = AssetUtils.RemoveFileExtension(fileName);
                var data = AssetUtils.GetScriptableObject<ExplodeData>(
                    ExplodeData.ASSET_PATH, fileName, false, false);
                if (data != null) {
                    _explodeDatas.Add(data);
                }
            }

            for (int i = 0; i < _explodeDatas.Count; i++) {
                ExplodeData data = _explodeDatas[i];
                if (_activeExplodeDataIndex == i) {
                    _foldouts.Add(false);
                }
                else {
                    _foldouts.Add(true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void Update() {
            _scaleW = (float)Screen.width / 800;     //计算宽度缩放比
            _scaleH = (float)Screen.height / 480;    //计算高度缩放比
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnGUI() {
            int oldFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = (int)(25 * _scaleW);

            if (_greenTeamRoot == null) {
                if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), "Create")) {
                    InitializeBuild(CurrentExplodeIndex());
                }

                return;
            }


            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), "Attack")) {


            }

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH * 2, 120 * _scaleW, 40 * _scaleH), "Destory")) {

                // 删除 绿队
                _greenTeamRoot.transform.DetachChildren();
                for (int i = 0; i < _greenTeams.Count; i++) {
                    Destroy(_greenTeams[i].gameObject);
                }
                Destroy(_greenTeamRoot);
                _greenTeams.Clear();

                // 激活触发器
                var triggers = GameObject.Find("Triggers");
                if (triggers != null) {
                    triggers.transform.Find("CubeTrigger").gameObject.SetActive(true);
                }

                // 全局配置清零
                GlobalConfig.Instance.Clear();
            }

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH * 3, 120 * _scaleW, 40 * _scaleH), "Save")) {

                string fileName = EditorUtility.SaveFilePanel("Save To Json File", "", "", "");
                string fileNameAttack = fileName + "_attack.json";
                GameObject.Find("Yellow Team").GetComponent<CollsionSample>().SaveToFile(fileNameAttack);
                string fileNameDefense = fileName + "_defense.json";
                GameObject.Find("Green Team").GetComponent<CollsionSample>().SaveToFile(fileNameDefense);
            }


            GUI.skin.button.fontSize = oldFontSize;
        }
        #endregion

        #region (Methods) Initializations
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CurrentExplodeIndex() {
            return _activeExplodeDataIndex % _explodeDatas.Count;
        }


        /// <summary>
        /// /
        /// </summary>
        /// <param name="assaultIdx"></param>
        public void InitializeBuild(int dataIdx) {
            
            if (_greenTeamPrefab == null) {
                throw new ArgumentNullException("Yellow team prefab is null!");
            }

            if (_greenTeamRoot == null) {
                _greenTeamRoot = new GameObject("Green Team");
                _greenTeamRoot.AddComponent<AssaultDefenseCollisionSample>();
                _greenTeamRoot.transform.position = Vector3.zero;
            }

            ExplodeData curExplodeData = _explodeDatas[dataIdx];
            if (curExplodeData == null) {
                throw new ArgumentNullException("Current assault data is null!");
            }

            TeamData greenTeamData = curExplodeData._teamData;
            if (greenTeamData._writeBack) {
                InitializeGreenTeam(greenTeamData);
            }
            else {
                InitializeDefaultGreenTeam(greenTeamData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void InitializeDefaultGreenTeam(TeamData teamData) {
            // Check
            if (teamData._total <= 0) {
                throw new ArgumentException("Total must be greater than 0");
            }
            if (teamData._row <= 0) {
                throw new ArgumentException("Row must be greater than 0");
            }
            if (teamData._total < teamData._row) {
                throw new ArgumentException("The total must be greater than the number of rows");
            }

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            // Calculate Position
            float rowStartPos = (teamData._spanRow * (column - 1) / 2);
            float colStartPos = -(teamData._spanColumn * (teamData._row - 1) / 2);

            int count = 0;
            for (int rowIdx = 0; rowIdx < teamData._row; rowIdx++) {
                for (int columnIdx = 0; columnIdx < column; columnIdx++) {
                    if (count >= teamData._total)
                        break;

                    var x = rowStartPos - (columnIdx * teamData._spanRow);
                    var z = colStartPos + (rowIdx * teamData._spanColumn);
                    var newObj = Instantiate(
                        _greenTeamPrefab, new Vector3(x, 0f, z), Quaternion.identity, _greenTeamRoot.transform);
                    newObj.name = string.Format("[{0}][{1}]", rowIdx, columnIdx);

                    // collider center
                    if (newObj.GetComponent<Collider>() is BoxCollider) {
                        newObj.GetComponent<BoxCollider>().center = teamData._colliders[rowIdx]._centre;
                    }
                    else {
                        newObj.GetComponent<CapsuleCollider>().center = teamData._colliders[rowIdx]._centre;
                    }

                    // collider size
                    if (newObj.GetComponent<Collider>() is BoxCollider) {
                        newObj.GetComponent<BoxCollider>().size = teamData._colliders[rowIdx]._size;
                    }
                    else {
                        newObj.GetComponent<CapsuleCollider>().radius = teamData._colliders[rowIdx]._size.x;
                        newObj.GetComponent<CapsuleCollider>().height = teamData._colliders[rowIdx]._size.y;
                    }

                    // rigid mass
                    newObj.GetComponent<Rigidbody>().mass = teamData._rigids[rowIdx]._mass;

                    // rigid drag
                    newObj.GetComponent<Rigidbody>().drag = teamData._rigids[rowIdx]._drag;

                    _greenTeams.Add(newObj.transform);
                    count++;
                }
            }

            _greenTeamRoot.transform.position = teamData._worldPosition;
            SetGoLayers(_greenTeamRoot, LayerMask.NameToLayer("GreenTeam"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void InitializeGreenTeam(TeamData teamData) {
            // Check
            if (teamData._total <= 0) {
                throw new ArgumentException("Total must be greater than 0");
            }
            if (teamData._row <= 0) {
                throw new ArgumentException("Row must be greater than 0");
            }
            if (teamData._total < teamData._row) {
                throw new ArgumentException("The total must be greater than the number of rows");
            }

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            if (!teamData._writeBack) {
                return;
            }

            for (int i = 0; i < teamData._localPositions.Count; i++) {
                Vector3 pos = teamData._localPositions[i];
                Quaternion quater = teamData._localRotations[i];
                var newObj = Instantiate(
                    _greenTeamPrefab, pos, quater, _greenTeamRoot.transform);
                newObj.name = string.Format("[{0}][{1}]", i / column, i % column);

                int rowIdx = i / column;

                // collider center
                if (newObj.GetComponent<Collider>() is BoxCollider) {
                    newObj.GetComponent<BoxCollider>().center = teamData._colliders[rowIdx]._centre;
                }
                else {
                    newObj.GetComponent<CapsuleCollider>().center = teamData._colliders[rowIdx]._centre;
                }

                // collider size
                if (newObj.GetComponent<Collider>() is BoxCollider) {
                    newObj.GetComponent<BoxCollider>().size = teamData._colliders[rowIdx]._size;
                }
                else {
                    newObj.GetComponent<CapsuleCollider>().radius = teamData._colliders[rowIdx]._size.x;
                    newObj.GetComponent<CapsuleCollider>().height = teamData._colliders[rowIdx]._size.y;
                }

                // rigid mass
                newObj.GetComponent<Rigidbody>().mass = teamData._rigids[rowIdx]._mass;

                // rigid drag
                newObj.GetComponent<Rigidbody>().drag = teamData._rigids[rowIdx]._drag;

                _greenTeams.Add(newObj.transform);
            }

            _greenTeamRoot.transform.position = teamData._worldPosition;
            SetGoLayers(_greenTeamRoot, LayerMask.NameToLayer("GreenTeam"));
        }
        #endregion

        #region (Methods) Apply 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="explodeIdx"></param>
        /// <param name="teamIdx"></param>
        public void ApplyArrangeGreenTeam(int explodeIdx, int teamIdx) {

            ExplodeData curExplodeData = _explodeDatas[explodeIdx];
            if (curExplodeData == null) {
                throw new ArgumentNullException("Current assault data is null!");
            }

            TeamData teamData = curExplodeData._teamData;
            if (teamData == null) {
                throw new ArgumentNullException("Current team data is null!");
            }

            if (!teamData._writeBack) {
                teamData._localPositions = new List<Vector3>();
                teamData._localRotations = new List<Quaternion>();

                for (int i = 0; i < _greenTeams.Count; i++) {
                    teamData._localPositions.Add(_greenTeams[i].localPosition);
                    teamData._localRotations.Add(_greenTeams[i].localRotation);
                }
                teamData._writeBack = true;
            }
            else {
                for (int i = 0; i < _greenTeams.Count; i++) {
                    teamData._localPositions[i] = _greenTeams[i].localPosition;
                    teamData._localRotations[i] = _greenTeams[i].localRotation;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamIdx"></param>
        /// <param name="rowIdex"></param>
        public void ApplyCustomMass(int rowIdex, float mass) {

            int dataIdx = CurrentExplodeIndex();
            var explodeData = _explodeDatas[dataIdx];

            var teamData = explodeData._teamData;

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                _greenTeams[i].GetComponent<Rigidbody>().mass = mass;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ApplyCustomDrag(int teamIdx, int rowIdex, float drag) {
            int dataIdx = CurrentExplodeIndex();
            var explodeData = _explodeDatas[dataIdx];

            var teamData = explodeData._teamData;

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                _greenTeams[i].GetComponent<Rigidbody>().drag = drag;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ApplyCustomColliderCenter(int teamIdx, int rowIdex, Vector3 center) {
            int dataIdx = CurrentExplodeIndex();
            var explodeData = _explodeDatas[dataIdx];

            var teamData = explodeData._teamData;

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                if (_greenTeams[i].GetComponent<Collider>() is BoxCollider) {
                    _greenTeams[i].GetComponent<BoxCollider>().center = center;
                }
                else {
                    _greenTeams[i].GetComponent<CapsuleCollider>().center = center;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamIdx"></param>
        /// <param name="rowIdex"></param>
        /// <param name="center"></param>
        public void ApplyCustomColliderSize(int teamIdx, int rowIdex, Vector3 size) {
            int dataIdx = CurrentExplodeIndex();
            var explodeData = _explodeDatas[dataIdx];

            var teamData = explodeData._teamData;

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                if (_greenTeams[i].GetComponent<Collider>() is BoxCollider) {
                    _greenTeams[i].GetComponent<BoxCollider>().size = size;
                }
                else {
                    _greenTeams[i].GetComponent<CapsuleCollider>().radius = size.x;
                    _greenTeams[i].GetComponent<CapsuleCollider>().height = size.y;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion

        #region (Methods) Tools
        private void SetGoLayers(GameObject go, int layer) {
            try {
                var childrenTrans = go.GetComponentsInChildren<Transform>();
                for (int i = 0; i < childrenTrans.Length; i++) {
                    childrenTrans[i].gameObject.layer = layer;
                }
            }
            catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }
        #endregion

        #region 
        public List<Transform> GetGreenTeams() {
            return _greenTeams;
        }
        #endregion
    }
}