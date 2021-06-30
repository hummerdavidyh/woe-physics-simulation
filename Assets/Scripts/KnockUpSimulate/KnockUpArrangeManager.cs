using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    public class KnockUpArrangeManager : MonoBehaviourSingleton<KnockUpArrangeManager> {
        #region (Fields)
        /// <summary> 宽度缩放比 </summary>
        private float _scaleW = 1f;
        /// <summary> 高度缩放比 </summary>
        private float _scaleH = 1f;
        #endregion

        #region 
        /// <summary>  </summary>
        public KnockUpDatabase _database;
        /// <summary>  </summary>
        public int _currentDataIndex;

        /// <summary>  </summary>
        public KnockUpColliders _attackColliders;
        /// <summary>  </summary>
        public int _currentAttackColliderIndex;
        #endregion

        #region (Fields)
        /// <summary> 防御队列预制体 </summary>
        public GameObject _defensiveTeamPrefab;
        /// <summary>  </summary>
        private Transform _defensiveTeamRoot;
        /// <summary>  </summary>
        private List<Transform> _defensiveTeams = null;

        /// <summary>  </summary>
        private Transform _attackTeamRoot;
        /// <summary>  </summary>
        private List<Transform> _attackTeams = null;

        /// <summary>  </summary>
        public float _attackSpeed;
        #endregion

        #region (Properties)
        /// <summary>
        /// 
        /// </summary>
        public KnockUpData CurrentKnockupData {
            get {
                int idx = _currentDataIndex % _database.Count;
                return _database.Get(idx);
            }
        }
        #endregion

        #region 
        // Start is called before the first frame update
        void Start() {
            Debug.Log("KnockUp DataBase " + _database.Count);
            _defensiveTeams = new List<Transform>();
            _attackTeams = new List<Transform>();
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnGUI() {
            int oldFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = (int)(25 * _scaleW);

            if (_defensiveTeamRoot == null || _attackTeamRoot == null) {
                if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), "Create")) {
                    InitializeBuild();
                }
                return;
            }


            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), "Attack")) {
                Vector3 direction = (_defensiveTeamRoot.transform.position - _attackTeamRoot.transform.position).normalized;
                foreach (var item in _attackTeams) {
                    item.GetComponent<Rigidbody>().velocity = direction * _attackSpeed;
                    //item.GetComponent<Rigidbody>().AddForce(direction * _attackSpeed, ForceMode.Force);
                }
            }

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH * 2, 120 * _scaleW, 40 * _scaleH), "Destory")) {

                // 删除Defense队列
                _defensiveTeamRoot.transform.DetachChildren();
                for (int i = 0; i < _defensiveTeams.Count; i++) {
                    Destroy(_defensiveTeams[i].gameObject);
                }
                Destroy(_defensiveTeamRoot.gameObject);
                _defensiveTeams.Clear();

                // 删除Attack队列
                _attackTeamRoot.transform.DetachChildren();
                for (int i = 0; i < _attackTeams.Count; i++) {
                    Destroy(_attackTeams[i].gameObject);
                }
                Destroy(_attackTeamRoot.gameObject);
                _attackTeams.Clear();


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

        #region 
        public void InitializeBuild() {

            // 初始化DefensiveTeam
            if (_defensiveTeamPrefab == null) {
                throw new ArgumentNullException("Defense Team prefab(Green Team) is null!");
            }

            if (_defensiveTeamRoot == null) {
                _defensiveTeamRoot = new GameObject("Defense Team (Green Team)").transform;
                //_defensiveTeamRoot.gameObject.AddComponent<AssaultDefenseCollisionSample>();
                _defensiveTeamRoot.transform.position = Vector3.zero;
            }

            InitializeDefaultDefenseTeam(CurrentKnockupData._teamData);

            // 初始化AttackTeam
            if (_attackTeamRoot == null) {
                _attackTeamRoot = new GameObject("Attack Team (Yellow Team)").transform;
                _attackTeamRoot.transform.position = Vector3.zero;
            }

            InitializeDefaultAttackTeam();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void InitializeDefaultDefenseTeam(TeamData teamData) {
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
                        _defensiveTeamPrefab, new Vector3(x, 0f, z), Quaternion.identity, _defensiveTeamRoot.transform);
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

                    _defensiveTeams.Add(newObj.transform);
                    count++;
                }
            }

            _defensiveTeamRoot.transform.position = teamData._worldPosition;
            SetGoLayers(_defensiveTeamRoot.gameObject, LayerMask.NameToLayer("GreenTeam"));
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeDefaultAttackTeam() {
            if (_attackTeams == null) {
                _attackTeams = new List<Transform>();
            }

            int idx = _currentAttackColliderIndex % _attackColliders.Count;
            var collider = _attackColliders.Get(idx).gameObject;
            collider.transform.SetParent(_attackTeamRoot);
            collider.transform.localPosition = new Vector3(0f, 1f, -10f);

            _attackTeams.Add(collider.gameObject.transform);

            SetGoLayers(_attackTeamRoot.gameObject, LayerMask.NameToLayer("YellowTeam"));
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
    }
}