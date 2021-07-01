using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    public class KnockUpArrangeManager : ArrangeManager {
  
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

      
        #region (Properties)
        /// <summary>  </summary>
        public KnockUpData CurrentKnockupData {
            get {
                int idx = _currentDataIndex % _database.Count;
                return _database.Get(idx);
            }
        }

        /// <summary>  </summary>
        protected override TeamData AttackTeamData => null;

        /// <summary>  </summary>
        protected override TeamData DefenseTeamData => CurrentKnockupData._defenseTeamData;
        #endregion

        #region 
        public override void InitializeBuild() {
            if (_defenseTeamPrefab == null) {
                throw new ArgumentNullException("Defense team prefab is null!");
            }

            if (_defenseTeamRoot == null) {
                _defenseTeamRoot = new GameObject("Defense Team (Green Team)").transform;
            }
            _defenseTeamRoot.transform.position = Vector3.zero;
            InitializeDefaultDefenseTeam();
            _defenseTeamRoot.gameObject.AddComponent<DefenseCollisionSample>();

            // ≥ı ºªØAttackTeam
            if (_attackTeamRoot == null) {
                _attackTeamRoot = new GameObject("Attack Team (Yellow Team)").transform;
            }
            _attackTeamRoot.transform.position = Vector3.zero;

            InitializeDefaultAttackTeam();
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeDefaultAttackTeam() {
            if (_attackTeams == null) {
                _attackTeams = new List<Transform>();
            }

            int idx = _currentAttackColliderIndex % _attackColliders.Count;
            var collider = _attackColliders.Get(idx).gameObject;
            collider.transform.SetParent(_attackTeamRoot);
            _attackTeamRoot.position = CurrentKnockupData._attackPosition;

            _attackTeams.Add(collider.gameObject.transform);

            SetGoLayers(_attackTeamRoot.gameObject, LayerMask.NameToLayer("AttackTeam"));
        }
        #endregion

        #region (OnGUI Methods)
        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttackButton() {
            Vector3 direction = (_defenseTeamRoot.position - _attackTeamRoot.position).normalized;
            foreach (var item in _attackTeams) {
                item.GetComponent<Rigidbody>().velocity = direction * _attackSpeed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnSaveButton() {
            string fileName = EditorUtility.SaveFilePanel("Save To Json File", "", "", "");
            string fileNameDefense = fileName + "_defense.json";
            _defenseTeamRoot.gameObject.GetComponent<DefenseCollisionSample>().SaveToFile(fileNameDefense);
        }
        #endregion
    }
}