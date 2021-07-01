using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    /// <summary>
    /// 
    /// </summary>
    public class ArrangeManager : MonoBehaviour {
        #region (Const Fields)
        private const string ONGUI_CREATE_NAME = "Create";
        private const string ONGUI_ATTACK_NAME = "Attack";
        private const string ONGUI_DESTORY_NAME = "Destroy";
        private const string ONGUI_SAVE_NAME = "Save";

        #endregion

        #region (Fields) Prefabs
        /// <summary>  </summary>
        
        [Header("攻击队列的预制体")]
        public Transform _attackTeamPrefab;
        /// <summary>  </summary>
        [Header("攻击队列的预制体")]
        [Space(1)]
        public Transform _defenseTeamPrefab;
        #endregion

        #region (Fields) Teams
        /// <summary> </summary>
        protected Transform _attackTeamRoot;
        /// <summary> </summary>
        protected Transform _defenseTeamRoot;
        /// <summary> </summary>
        protected List<Transform> _attackTeams = new List<Transform>();
        /// <summary> </summary>
        protected List<Transform> _defenseTeams = new List<Transform>();
        #endregion

        #region (Fields) Attck
        /// <summary> </summary>
        [Header("攻击速度")]
        [Space(1)]
        [Range(1, 1000)]
        public float _attackSpeed;
        /// <summary> </summary>
        [Header("碰撞触发器")]
        [Space(1)]
        public GameObject _trigger;

        /// <summary> 宽度缩放比 </summary>
        protected float _scaleW = 1.0f;
        /// <summary> 高度缩放比 </summary>
        protected float _scaleH = 1.0f;
        #endregion

        #region (Properties)
        /// <summary>  </summary>
        protected virtual TeamData DefenseTeamData { 
            get; 
            private set; 
        }

        /// <summary> </summary>
        protected virtual TeamData AttackTeamData { 
            get; 
            private set; 
        }
        #endregion

        #region (Unity Methods)
        // Start is called before the first frame update
        protected virtual void Start() {
        }

        // Update is called once per frame
        protected virtual void Update() {
            _scaleW = (float)Screen.width / 800;     //计算宽度缩放比
            _scaleH = (float)Screen.height / 480;    //计算高度缩放比
        }

        private void OnGUI() {
            int oldFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = (int)(25 * _scaleW);

            if (_attackTeamRoot == null || _defenseTeamRoot == null) {
                if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), ONGUI_CREATE_NAME)) {
                    InitializeBuild();
                }
                return;
            }


            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), ONGUI_ATTACK_NAME)) {
                OnAttackButton();
            }

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH * 2, 120 * _scaleW, 40 * _scaleH), ONGUI_DESTORY_NAME)) {
                OnDestoryButton();
            }

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH * 3, 120 * _scaleW, 40 * _scaleH), ONGUI_SAVE_NAME)) {
                OnSaveButton();
            }

            GUI.skin.button.fontSize = oldFontSize;
        }
        #endregion

        #region (Methods) Initialization
        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeBuild() {
            if (_attackTeamPrefab == null) {
                throw new ArgumentNullException("Yellow team prefab is null!");
            }

            if (_defenseTeamPrefab == null) {
                throw new ArgumentNullException("Yellow team prefab is null!");
            }

            if (_attackTeamRoot == null) {
                _attackTeamRoot = new GameObject("AttackTeam").transform;
                _attackTeamRoot.gameObject.AddComponent<AttackCollisionSample>();
                _attackTeamRoot.transform.position = Vector3.zero;
            }

            if (_defenseTeamRoot == null) {
                _defenseTeamRoot = new GameObject("Green Team").transform;
                _defenseTeamRoot.gameObject.AddComponent<DefenseCollisionSample>();
                _defenseTeamRoot.transform.position = Vector3.zero;
            }
        } 

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeDefaultAttackTeam() {
            // Check
            if (AttackTeamData._total <= 0) {
                throw new ArgumentException("Total must be greater than 0");
            }
            if (AttackTeamData._row <= 0) {
                throw new ArgumentException("Row must be greater than 0");
            }
            if (AttackTeamData._total < AttackTeamData._row) {
                throw new ArgumentException("The total must be greater than the number of rows");
            }

            // Calculate Column
            int column = AttackTeamData._total / AttackTeamData._row;
            if (AttackTeamData._total % AttackTeamData._row != 0) {
                column += 1;
            }

            // Calculate Position
            float rowStartPos = -(AttackTeamData._spanRow * (column - 1) / 2);
            float colStartPos = (AttackTeamData._spanColumn * (AttackTeamData._row - 1) / 2);

            int count = 0;

            for (int rowIdx = 0; rowIdx < AttackTeamData._row; rowIdx++) {
                for (int columnIdx = 0; columnIdx < column; columnIdx++) {
                    if (count >= AttackTeamData._total)
                        break;

                    var x = rowStartPos + (columnIdx * AttackTeamData._spanRow);
                    var z = colStartPos - (rowIdx * AttackTeamData._spanColumn);
                    var newObj = Instantiate(
                        _attackTeamPrefab, new Vector3(x, 0f, z), Quaternion.identity, _attackTeamRoot.transform);
                    newObj.name = string.Format("[{0}][{1}]", rowIdx, columnIdx);

                    // collider center
                    if (newObj.GetComponent<Collider>() is BoxCollider) {
                        newObj.GetComponent<BoxCollider>().center = AttackTeamData._colliders[rowIdx]._centre;
                    }
                    else {
                        newObj.GetComponent<CapsuleCollider>().center = AttackTeamData._colliders[rowIdx]._centre;
                    }

                    // collider size
                    if (newObj.GetComponent<Collider>() is BoxCollider) {
                        newObj.GetComponent<BoxCollider>().size = AttackTeamData._colliders[rowIdx]._size;
                    }
                    else {
                        newObj.GetComponent<CapsuleCollider>().radius = AttackTeamData._colliders[rowIdx]._size.x;
                        newObj.GetComponent<CapsuleCollider>().height = AttackTeamData._colliders[rowIdx]._size.y;
                    }

                    // rigid mass
                    newObj.GetComponent<Rigidbody>().mass = AttackTeamData._rigids[rowIdx]._mass;

                    // rigid drag
                    newObj.GetComponent<Rigidbody>().drag = AttackTeamData._rigids[rowIdx]._drag;

                    _attackTeams.Add(newObj.transform);
                }
            }

            _attackTeamRoot.position = AttackTeamData._worldPosition;
            SetGoLayers(_attackTeamRoot.gameObject, LayerMask.NameToLayer("AttackTeam"));
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeDefaultDefenseTeam() {
            // Check
            if (DefenseTeamData._total <= 0) {
                throw new ArgumentException("Default defenseTeamData's total must be greater than 0!");
            }
            if (DefenseTeamData._row <= 0) {
                throw new ArgumentException("Default defenseTeamData's row must be greater than 0!");
            }
            if (DefenseTeamData._total < DefenseTeamData._row) {
                throw new ArgumentException("Default defenseTeamData's total must be greater than the number of rows!");
            }

            // Calculate Column
            int column = DefenseTeamData._total / DefenseTeamData._row;
            if (DefenseTeamData._total % DefenseTeamData._row != 0) {
                column += 1;
            }

            // Calculate Position
            float rowStartPos = (DefenseTeamData._spanRow * (column - 1) / 2);
            float colStartPos = -(DefenseTeamData._spanColumn * (DefenseTeamData._row - 1) / 2);

            int count = 0;
            for (int rowIdx = 0; rowIdx < DefenseTeamData._row; rowIdx++) {
                for (int columnIdx = 0; columnIdx < column; columnIdx++) {
                    if (count >= DefenseTeamData._total)
                        break;

                    var x = rowStartPos - (columnIdx * DefenseTeamData._spanRow);
                    var z = colStartPos + (rowIdx * DefenseTeamData._spanColumn);
                    var newObj = Instantiate(
                        _defenseTeamPrefab, new Vector3(x, 0f, z), Quaternion.identity, _defenseTeamRoot.transform);
                    newObj.name = string.Format("[{0}][{1}]", rowIdx, columnIdx);

                    // collider center
                    if (newObj.GetComponent<Collider>() is BoxCollider) {
                        newObj.GetComponent<BoxCollider>().center = DefenseTeamData._colliders[rowIdx]._centre;
                    }
                    else {
                        newObj.GetComponent<CapsuleCollider>().center = DefenseTeamData._colliders[rowIdx]._centre;
                    }

                    // collider size
                    if (newObj.GetComponent<Collider>() is BoxCollider) {
                        newObj.GetComponent<BoxCollider>().size = DefenseTeamData._colliders[rowIdx]._size;
                    }
                    else {
                        newObj.GetComponent<CapsuleCollider>().radius = DefenseTeamData._colliders[rowIdx]._size.x;
                        newObj.GetComponent<CapsuleCollider>().height = DefenseTeamData._colliders[rowIdx]._size.y;
                    }

                    // rigid mass
                    newObj.GetComponent<Rigidbody>().mass = DefenseTeamData._rigids[rowIdx]._mass;

                    // rigid drag
                    newObj.GetComponent<Rigidbody>().drag = DefenseTeamData._rigids[rowIdx]._drag;

                    _defenseTeams.Add(newObj.transform);
                    count++;
                }
            }

            _defenseTeamRoot.position = DefenseTeamData._worldPosition;
            SetGoLayers(_defenseTeamRoot.gameObject, LayerMask.NameToLayer("DefenseTeam"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        protected void SetGoLayers(GameObject go, int layer) {
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
        ///
        protected virtual void OnAttackButton() {

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestoryButton() {
            // 删除进攻队伍
            _attackTeamRoot.DetachChildren();
            for (int i = 0; i < _attackTeams.Count; i++) {
                Destroy(_attackTeams[i].gameObject);
            }
            Destroy(_attackTeamRoot.gameObject);
            _attackTeams.Clear();

            // 删除防御队伍
            _defenseTeamRoot.DetachChildren();
            for (int i = 0; i < _defenseTeams.Count; i++) {
                Destroy(_defenseTeams[i].gameObject);
            }
            Destroy(_defenseTeamRoot.gameObject);
            _defenseTeams.Clear();

            // 激活触发器
            if (_trigger != null) {
                _trigger.SetActive(true);
            }

            // 全局配置清零
            GlobalConfig.Instance.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnSaveButton() {
            string fileName = EditorUtility.SaveFilePanel("Save To Json File", "", "", "");
            string fileNameAttack = fileName + "_attack.json";
            _attackTeamRoot.GetComponent<CollsionSample>().SaveToFile(fileNameAttack);
            string fileNameDefense = fileName + "_defense.json";
            _defenseTeamRoot.GetComponent<CollsionSample>().SaveToFile(fileNameDefense);
        }
        #endregion
    }
}