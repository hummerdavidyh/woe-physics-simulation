using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class AssaultArrangeManager : ArrangeManager {

        #region (Fields) Assault Database
        /// <summary>  </summary>
        [Header("当前突击数据索引号")]
        [Space(1)]
        public int _currentAssaultDataIndex;
        /// <summary>  </summary>
        [Header("突击数据库")]
        [Space(1)]
        public AssaultDatabase _assaultDatabase;
        #endregion

        #region (Properties)
        /// <summary> </summary>
        protected override TeamData DefenseTeamData {
            get {
                return CurrentAssaultData._defenseTeamData;
            }
        }

        /// <summary> </summary>
        protected override TeamData AttackTeamData {
            get {
                return CurrentAssaultData._attackTeamData;
            }
        }

        /// <summary> </summary>
        private AssaultData CurrentAssaultData {
            get {
                int idx = _currentAssaultDataIndex % _assaultDatabase.Count;
                return _assaultDatabase.Get(idx);
            }
        }
        #endregion

        #region 
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        protected override void Start() {
            base.Start();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update() {
            base.Update();
        }
        #endregion

        #region (Methods) Initialization
        /// <summary>
        /// 
        /// </summary>
        public override void InitializeBuild() {
            base.InitializeBuild();

            AssaultData curAssaultData = CurrentAssaultData;
            if (curAssaultData == null) {
                throw new ArgumentNullException("Current assault data is null!");
            }

            if (AttackTeamData._writeBack) {
                InitializeAttackTeam();
            }
            else {
                InitializeDefaultAttackTeam();
            }
            _attackTeamRoot.gameObject.AddComponent<AttackCollisionSample>();

            if (DefenseTeamData._writeBack) {
                InitializeDefeneseTeam();
            }
            else {
                InitializeDefaultDefenseTeam();
            }
            _defenseTeamRoot.gameObject.AddComponent<DefenseCollisionSample>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void InitializeAttackTeam() {         
            // Check
            if (AttackTeamData._total <= 0) {
                throw new ArgumentException("AttackTeamData's total must be greater than 0");
            }
            if (AttackTeamData._row <= 0) {
                throw new ArgumentException("AttackTeamData's row must be greater than 0");
            }
            if (AttackTeamData._total < AttackTeamData._row) {
                throw new ArgumentException("AttackTeamData's total must be greater than the number of rows");
            }

            // Calculate Column
            int column =  AttackTeamData._total / AttackTeamData._row;
            if (AttackTeamData._total % AttackTeamData._row != 0) {
                column += 1;
            }

            if (!AttackTeamData._writeBack) {
                return;
            }

            for (int i = 0; i < AttackTeamData._localPositions.Count; i++) {
                Vector3 pos = AttackTeamData._localPositions[i];
                Quaternion quater = AttackTeamData._localRotations[i];
                var newObj = Instantiate(
                    _attackTeamPrefab, pos, quater, _attackTeamRoot.transform);
                newObj.name = string.Format("[{0}][{1}]", i / column, i % column);

                int rowIdx = i / column;

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

            _attackTeamRoot.position = AttackTeamData._worldPosition;
            SetGoLayers(_attackTeamRoot.gameObject, LayerMask.NameToLayer("AttackTeam"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void InitializeDefeneseTeam() {
            // Check
            if (DefenseTeamData._total <= 0) {
                throw new ArgumentException("DefenseTeamData's total must be greater than 0!");
            }
            if (DefenseTeamData._row <= 0) {
                throw new ArgumentException("DefenseTeamData's row must be greater than 0!");
            }
            if (DefenseTeamData._total < DefenseTeamData._row) {
                throw new ArgumentException("DefenseTeamData's total must be greater than the number of rows!");
            }

            // Calculate Column
            int column = DefenseTeamData._total / DefenseTeamData._row;
            if (DefenseTeamData._total % DefenseTeamData._row != 0) {
                column += 1;
            }

            if (!DefenseTeamData._writeBack) {
                return;
            }

            for (int i = 0; i < DefenseTeamData._localPositions.Count; i++) {
                Vector3 pos = DefenseTeamData._localPositions[i];
                Quaternion quater = DefenseTeamData._localRotations[i];
                var newObj = Instantiate(
                    _defenseTeamPrefab, pos, quater, _defenseTeamRoot.transform);
                newObj.name = string.Format("[{0}][{1}]", i / column, i % column);

                int rowIdx = i / column;

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
            }

            _defenseTeamRoot.position = DefenseTeamData._worldPosition;
            SetGoLayers(_defenseTeamRoot.gameObject, LayerMask.NameToLayer("DefenseTeam"));
        }
        #endregion

        #region 
        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttackButton() {
            base.OnAttackButton();

            Vector3 direction;

            direction = (_defenseTeamRoot.transform.position - _attackTeamRoot.transform.position).normalized;
            Debug.Log("attack direction" + direction.x + direction.y + direction.z);

            foreach (var item in _attackTeams) {
                item.GetComponent<Rigidbody>().velocity = direction * _attackSpeed;
            }
        }
        #endregion
    }
}
