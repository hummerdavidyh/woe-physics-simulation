using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public enum TeamType {        
        Attack = 0,
        Defense,
        MaxType,
    }

    public class AssaultData : ScriptableObject  {
        #region (Const Fields)
        [NonSerialized]
        public const int DEFAULT_TEAM_TOTAL = 50;
        /// <summary> </summary>
        [NonSerialized]
        public const int DEFAULT_TEAM_ROW_NUM = 5;
        /// <summary> </summary>
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_ROW = 1f;
        /// <summary> </summary>
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_COLUMN = 1f;
        /// <summary> </summary>
        [NonSerialized]
        public const string ASSET_PATH = "Assets/Resources/DatabaseAssault";
        /// <summary> </summary>
        [NonSerialized]
        public const string ASSET_NAME = "defaultAssault";
        #endregion

        #region (Fields) Team Data
        /// <summary> </summary>
        [SerializeField]
        public TeamData _attackTeamData;
        /// <summary> </summary>
        [SerializeField]
        public TeamData _defenseTeamData;
        #endregion

        #region (Methods)
        [MenuItem("GoldLion/Assets/AssaultData")]
        static void CreateAssetInstance() {
            if (!Directory.Exists(ASSET_PATH)) {
                Directory.CreateDirectory(ASSET_PATH);
            }

            AssaultData inst = CreateInstance<AssaultData>();
            string FileName = string.Format("{0}/{1}.asset", ASSET_PATH, ASSET_NAME);
            AssetDatabase.CreateAsset(inst, FileName);

            inst._attackTeamData = CreateTeameData("Attack Team", FileName);
            inst._defenseTeamData = CreateTeameData("Default Team", FileName);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FileName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assetFileName"></param>
        /// <returns></returns>
        static TeamData CreateTeameData(string name, string assetFileName) {
            var teamData = CreateInstance<TeamData>();

            teamData.name = name;
            teamData._total = DEFAULT_TEAM_TOTAL;
            teamData._row = DEFAULT_TEAM_ROW_NUM;
            teamData._spanRow = DEFAULT_TEAM_SPAW_ROW;
            teamData._spanColumn = DEFAULT_TEAM_SPAW_COLUMN;

            // Collider
            teamData._colliders = new List<ColliderData>();
            for (int k = 0; k < teamData._row; k++) {
                var data = new ColliderData();
                data._centre = new Vector3(0f, 0.25f, 0f);
                data._size = new Vector3(0.5f, 0.5f, 0.5f);

                teamData._colliders.Add(data);
            }

            // Rigid
            teamData._rigids = new List<RigidData>();
            for (int k = 0; k < teamData._row; k++) {
                var data = new RigidData();
                data._mass = 15f;
                data._drag = 0.5f;

                teamData._rigids.Add(data);
            }

            AssetDatabase.AddObjectToAsset(teamData, assetFileName);

            return teamData;
        }
        #endregion

        #region (Unity Methods)
        /// <summary>
        /// 
        /// </summary>
        private void OnEnable() {
            Debug.Log(name + " ScriptableObject OnEnable!");

            if (_attackTeamData != null)
                ModifyData(_attackTeamData);
            if (_defenseTeamData != null)
                ModifyData(_defenseTeamData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamData"></param>
        private void ModifyData(TeamData teamData) {
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
