using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    /// <summary>
    /// 
    /// </summary>
    public class KnockUpData : ScriptableObject {

        #region (Const Fields)
        [NonSerialized]
        public const int DEFAULT_TEAM_TOTAL = 50;
        [NonSerialized]
        public const int DEFAULT_TEAM_ROW_NUM = 5;
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_ROW = 1f;
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_COLUMN = 1f;
        [NonSerialized]
        public const string ASSET_PATH = "Assets/Resources/DatabaseKnockUp";
        [NonSerialized]
        public const string ASSET_NAME = "defaultAssault";
        #endregion

        #region (Fields) Data
        /// <summary> / </summary>
        [SerializeField]
        public TeamData _defenseTeamData;

        /// <summary> / </summary>
        [SerializeField]
        public Vector3 _attackPosition;
        /// <summary> / </summary>
        [SerializeField]
        public Vector3 _attackRotation;
        #endregion

        #region (Methods)
        [MenuItem("GoldLion/Assets/KnockUpData")]
        static void CreateAssetInstance() {
            if (!Directory.Exists(ASSET_PATH)) {
                Directory.CreateDirectory(ASSET_PATH);
            }

            KnockUpData inst = CreateInstance<KnockUpData>();
            string FileName = string.Format("{0}/{1}.asset", ASSET_PATH, ASSET_NAME);
            AssetDatabase.CreateAsset(inst, FileName);

            // 队伍数据
            var teamData = CreateInstance<TeamData>();
            teamData.name = "Defense Team";
            teamData._total = DEFAULT_TEAM_TOTAL;
            teamData._row = DEFAULT_TEAM_ROW_NUM;
            teamData._spanRow = DEFAULT_TEAM_SPAW_ROW;
            teamData._spanColumn = DEFAULT_TEAM_SPAW_COLUMN;
            teamData._colliders = new List<ColliderData>();
            for (int k = 0; k < teamData._row; k++) {
                var data = new ColliderData();
                data._centre = new Vector3(0f, 0.25f, 0f);
                data._size = new Vector3(0.5f, 0.5f, 0.5f);

                teamData._colliders.Add(data);
            }

            teamData._rigids = new List<RigidData>();
            for (int k = 0; k < teamData._row; k++) {
                var data = new RigidData();
                data._mass = 15f;
                data._drag = 0.5f;

                teamData._rigids.Add(data);
            }
            teamData._worldPosition = new Vector3(0f, 0f, 0f);
            teamData._worldRotation = new Vector3(0f, 0f, 0f);
            AssetDatabase.AddObjectToAsset(teamData, FileName);
            inst._defenseTeamData = teamData;

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FileName);
            AssetDatabase.Refresh();
        }

        private void OnEnable() {
            Debug.Log(name + " ScriptableObject OnEnable!");
            if (_defenseTeamData == null) {
                return;
            }
            TeamData teamData = _defenseTeamData;

            // 修正_colliders的数量以匹配row
            if (teamData._row < teamData._colliders.Count) {
                int startPos = teamData._row - 1;
                int count = teamData._colliders.Count - teamData._row;
                teamData._colliders.RemoveRange(startPos, count);
            }
            else if (teamData._row > teamData._colliders.Count) {
                int count = teamData._row - teamData._colliders.Count;
                for (int k = 0; k < count; k++) {
                    var data = new ColliderData();
                    data._centre = new Vector3(0f, 0.25f, 0f);
                    data._size = new Vector3(0.5f, 0.5f, 0.5f);

                    teamData._colliders.Add(data);
                }
            }

            // 修正_rigids的数量以匹配row
            if (teamData._row < teamData._rigids.Count) {
                int startPos = teamData._row - 1;
                int count = teamData._rigids.Count - teamData._row;
                teamData._rigids.RemoveRange(startPos, count);
            }
            else if (teamData._row > teamData._rigids.Count) {
                int count = teamData._row - teamData._rigids.Count;
                for (int k = 0; k < count; k++) {
                    var data = new RigidData();
                    data._mass = 15f;
                    data._drag = 0.5f;

                    teamData._rigids.Add(data);
                }
            }
        }
        #endregion
    }
}