using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class ArrangeManager : MonoBehaviourSingleton<ArrangeManager> {
        #region (Variables) Datas
        /// <summary>  </summary>        
        public int _activeAssaultData;

        /// <summary>  </summary>
        public List<AssaultData> _assaultDatas = new List<AssaultData>();
        /// <summary>  </summary>
        [HideInInspector()]
        public List<bool> _foldouts = new List<bool>();
        #endregion

        #region (Varibles) Prefabs
        /// <summary>  </summary>
        public GameObject _yellowTeamPrefab;
        /// <summary>  </summary>
        public GameObject _greenTeamPrefab;
        #endregion
        
        #region (Varibles) Attck
        [Range(1, 100)]
        public float _attackSpeed;
        #endregion

        #region (Varibles) Teams
        /// <summary> </summary>
        private GameObject _yellowTeamRoot;
        /// <summary> </summary>
        private GameObject _greenTeamRoot;
        /// <summary> </summary>
        private List<Transform> _yellowTeams = new List<Transform>();
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

            string[] files = Directory.GetFiles(AssaultData.ASSET_PATH);
            foreach (var file in files) {
                string fileName = AssetUtils.GetFileName(file);
                if (fileName.Contains(".meta")) {
                    continue;
                }

                fileName = AssetUtils.RemoveFileExtension(fileName);
                var data = AssetUtils.GetScriptableObject<AssaultData>(
                    AssaultData.ASSET_PATH, fileName, false, false);
                if (data != null) {
                    _assaultDatas.Add(data);
                }
            }

            for (int i = 0; i < _assaultDatas.Count; i++) {
                AssaultData data = _assaultDatas[i];
                if (_activeAssaultData == i) {
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

            if (_yellowTeamRoot == null || _greenTeamRoot == null) {
                if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), "Create")) {
                    InitializeBuild(CurrentAssaultIndx());
                }

                return;
            }
            

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH, 120 * _scaleW, 40 * _scaleH), "Attack")) {

                int assaultIndex = CurrentAssaultIndx();
                AssaultData curData = _assaultDatas[assaultIndex];

                Vector3 direction = Vector3.zero;
                if (curData._teamTypes[0] == TeamType.Attack && curData._teamTypes[1] == TeamType.Defense) {
                    direction = (_greenTeamRoot.transform.position - _yellowTeamRoot.transform.position).normalized;
                    Debug.Log("direction 1" + direction.x + direction.y + direction.z);

                    foreach (var item in _yellowTeams) {
                        item.GetComponent<Rigidbody>().velocity = direction * _attackSpeed;
                    }
                }
                else if (curData._teamTypes[0] == TeamType.Defense && curData._teamTypes[1] == TeamType.Attack) {
                    direction = (_yellowTeamRoot.transform.position - _greenTeamRoot.transform.position).normalized;
                    Debug.Log("direction 2" + direction.x + direction.y + direction.z);

                    foreach (var item in _greenTeams) {
                        item.GetComponent<Rigidbody>().velocity = direction * _attackSpeed;
                    }
                }
            }

            if (GUI.Button(new Rect(70 * _scaleW, 50 * _scaleH * 2, 120 * _scaleW, 40 * _scaleH), "Destory")) {
                _yellowTeamRoot.transform.DetachChildren();
                for (int i = 0; i < _yellowTeams.Count; i++) {
                    Destroy(_yellowTeams[i].gameObject);
                }
                Destroy(_yellowTeamRoot);
                _yellowTeams.Clear();

                _greenTeamRoot.transform.DetachChildren();
                for (int i = 0; i < _greenTeams.Count; i++) {
                    Destroy(_greenTeams[i].gameObject);
                }
                Destroy(_greenTeamRoot);
                _greenTeams.Clear();

                var triggers = GameObject.Find("Triggers");
                if (triggers != null) {
                    triggers.transform.Find("CubeTrigger").gameObject.SetActive(true);
                }
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
        public int CurrentAssaultIndx() {
            return _activeAssaultData % _assaultDatas.Count;
        }
  

        /// <summary>
        /// /
        /// </summary>
        /// <param name="assaultIdx"></param>
        public void InitializeBuild(int assaultIdx) {
            
            if (_yellowTeamPrefab == null) {
                throw new ArgumentNullException("Yellow team prefab is null!");
            }

            if (_greenTeamPrefab == null) {
                throw new ArgumentNullException("Yellow team prefab is null!");
            }

            if (_yellowTeamRoot == null) {
                _yellowTeamRoot = new GameObject("Yellow Team");
                _yellowTeamRoot.AddComponent<CollsionSample>();
                _yellowTeamRoot.GetComponent<CollsionSample>().enabled = false;
                _yellowTeamRoot.transform.position = Vector3.zero;
            }

            if (_greenTeamRoot == null) {
                _greenTeamRoot = new GameObject("Green Team");
                _greenTeamRoot.AddComponent<CollsionSample>();
                _greenTeamRoot.GetComponent<CollsionSample>().enabled = false;
                _greenTeamRoot.transform.position = Vector3.zero;
            }

            AssaultData curAssaultData = _assaultDatas[assaultIdx];
            if (curAssaultData == null) {
                throw new ArgumentNullException("Current assault data is null!");
            }

            TeamData yellowTeamData = curAssaultData._teamDatas[0];
            if (yellowTeamData._writeBack) {
                InitializeYellowTeam(yellowTeamData);
            }
            else {
                InitializeDefaultYellowTeam(yellowTeamData);
            }

            TeamData greenTeamData = curAssaultData._teamDatas[1];
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
        private void InitializeDefaultYellowTeam(TeamData teamData) {
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
            float rowStartPos = -(teamData._spanRow * (column - 1) / 2);
            float colStartPos = -(teamData._spanColumn * (teamData._row - 1) / 2);

            int count = 0;

            for (int rowIdx = 0; rowIdx < teamData._row; rowIdx++) {
                for (int columnIdx = 0; columnIdx < column; columnIdx++) {
                    if (count >= teamData._total)
                        break;

                    var x = rowStartPos + (columnIdx * teamData._spanRow);
                    var z = colStartPos + (rowIdx * teamData._spanColumn);
                    var newObj = Instantiate(
                        _yellowTeamPrefab, new Vector3(x, 0f, z), Quaternion.identity, _yellowTeamRoot.transform);
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

                    _yellowTeams.Add(newObj.transform);
                }
            }

            _yellowTeamRoot.transform.position = teamData._worldPosition;
            _yellowTeamRoot.transform.Rotate(Vector3.up, teamData._worldRotation.y);
            SetGoLayers(_yellowTeamRoot, LayerMask.NameToLayer("YellowTeam"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void InitializeYellowTeam(TeamData teamData) {
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
                    _yellowTeamPrefab, pos, quater, _yellowTeamRoot.transform);
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

                _yellowTeams.Add(newObj.transform);
            }

            _yellowTeamRoot.transform.position = teamData._worldPosition;
            _yellowTeamRoot.transform.Rotate(Vector3.up, teamData._worldRotation.y);
            SetGoLayers(_yellowTeamRoot, LayerMask.NameToLayer("YellowTeam"));
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
            float rowStartPos = -(teamData._spanRow * (column - 1) / 2);
            float colStartPos = -(teamData._spanColumn * (teamData._row - 1) / 2);

            int count = 0;
            for (int rowIdx = 0; rowIdx < teamData._row; rowIdx++) {
                for (int columnIdx = 0; columnIdx < column; columnIdx++) {
                    if (count >= teamData._total)
                        break;

                    var x = rowStartPos + (columnIdx * teamData._spanRow);
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
            _greenTeamRoot.transform.Rotate(Vector3.up, teamData._worldRotation.y);
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
            _greenTeamRoot.transform.Rotate(Vector3.up, teamData._worldRotation.y);
            SetGoLayers(_greenTeamRoot, LayerMask.NameToLayer("GreenTeam"));
        }
        #endregion

        #region (Methods) Apply 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assaultIdx"></param>
        /// <param name="teamIdx"></param>
        public void ApplyCustomArrange(int assaultIdx, int teamIdx) {

            AssaultData curAssaultData = _assaultDatas[assaultIdx];
            if (curAssaultData == null) {
                throw new ArgumentNullException("Current assault data is null!");
            }

            TeamData teamData = curAssaultData._teamDatas[teamIdx];
            if (teamData == null) {
                throw new ArgumentNullException("Current team data is null!");
            }

            List<Transform> curTeam = _yellowTeams;
            if (teamIdx == 1)
                curTeam = _greenTeams;

            teamData._writeBack = true;
            teamData._localPositions = new List<Vector3>();
            teamData._localRotations = new List<Quaternion>();
            for (int i = 0; i < _greenTeams.Count; i++) {
                teamData._localPositions.Add(curTeam[i].localPosition);
                teamData._localRotations.Add(curTeam[i].localRotation);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamIdx"></param>
        /// <param name="rowIdex"></param>
        public void ApplyCustomMass(int teamIdx, int rowIdex, float mass) {
            List<Transform> teamTrans;
            if (teamIdx == 0) {
                teamTrans = _yellowTeams;
            }
            else {
                teamTrans = _greenTeams;
            }

            int assaultIdx = CurrentAssaultIndx();
            var assaultData = _assaultDatas[assaultIdx];

            var teamData = assaultData._teamDatas[teamIdx];

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                teamTrans[i].GetComponent<Rigidbody>().mass = mass;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ApplyCustomDrag(int teamIdx, int rowIdex, float drag) {
            List<Transform> teamTrans;
            if (teamIdx == 0) {
                teamTrans = _yellowTeams;
            }
            else {
                teamTrans = _greenTeams;
            }

            int assaultIdx = CurrentAssaultIndx();
            var assaultData = _assaultDatas[assaultIdx];

            var teamData = assaultData._teamDatas[teamIdx];

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                teamTrans[i].GetComponent<Rigidbody>().drag = drag;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ApplyCustomColliderCenter(int teamIdx, int rowIdex, Vector3 center) {
            List<Transform> teamTrans;
            if (teamIdx == 0) {
                teamTrans = _yellowTeams;
            }
            else {
                teamTrans = _greenTeams;
            }

            int assaultIdx = CurrentAssaultIndx();
            var assaultData = _assaultDatas[assaultIdx];

            var teamData = assaultData._teamDatas[teamIdx];

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                if (teamTrans[i].GetComponent<Collider>() is BoxCollider) {
                    teamTrans[i].GetComponent<BoxCollider>().center = center;
                }
                else {
                    teamTrans[i].GetComponent<CapsuleCollider>().center = center;
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
            List<Transform> teamTrans;
            if (teamIdx == 0) {
                teamTrans = _yellowTeams;
            }
            else {
                teamTrans = _greenTeams;
            }

            int assaultIdx = CurrentAssaultIndx();
            var assaultData = _assaultDatas[assaultIdx];

            var teamData = assaultData._teamDatas[teamIdx];

            // Calculate Column
            int column = teamData._total / teamData._row;
            if (teamData._total % teamData._row != 0) {
                column += 1;
            }

            for (int i = rowIdex * column; i < rowIdex * column + column; i++) {
                if (teamTrans[i].GetComponent<Collider>() is BoxCollider) {
                    teamTrans[i].GetComponent<BoxCollider>().size = size;
                }
                else {
                    teamTrans[i].GetComponent<CapsuleCollider>().radius = size.x;
                    teamTrans[i].GetComponent<CapsuleCollider>().height = size.y;
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
        public List<Transform> GetYellowTeams() {
            return _yellowTeams;
        }

        public List<Transform> GetGreenTeams() {
            return _greenTeams;
        }
        #endregion
    }
}
