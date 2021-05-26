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
        [NonSerialized]
        public const int DEFAULT_TEAMS_NUM = 2;
        [NonSerialized]
        public const int DEFAULT_TEAM_TOTAL = 50;
        [NonSerialized]
        public const int DEFAULT_TEAM_ROW_NUM = 5;
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_ROW = 1f;
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_COLUMN = 1f;
        [NonSerialized]
        public const string ASSET_PATH = "Assets/Resources/DatabaseAssault";
        [NonSerialized]
        public const string ASSET_NAME = "defaultAssault";

        [SerializeField]
        public List<TeamType> _teamTypes;
        [SerializeField]
        public List<TeamData> _teamDatas;
        [SerializeField]
        public List<bool> _foldouts;

        [MenuItem("GoldLion/Assets/AssaultData")]
        static void CreateAssetInstance() {
            if (!Directory.Exists(ASSET_PATH)) {
                Directory.CreateDirectory(ASSET_PATH);
            }

            AssaultData inst = CreateInstance<AssaultData>();
            string FileName = string.Format("{0}/{1}.asset", ASSET_PATH, ASSET_NAME);
            AssetDatabase.CreateAsset(inst, FileName);

            inst._teamTypes = new List<TeamType>();
            inst._teamDatas = new List<TeamData>();
            inst._foldouts = new List<bool>();

            for (int i = 0; i < DEFAULT_TEAMS_NUM; i++) {
                TeamType type = TeamType.MaxType;

                inst._teamTypes.Add(type);
                inst._foldouts.Add(false);

                var teamData = CreateInstance<TeamData>();
                teamData.name = "Team " + i.ToString();
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
                
                AssetDatabase.AddObjectToAsset(teamData, FileName);
                inst._teamDatas.Add(teamData);                
            }

            inst._teamTypes[0] = TeamType.Attack;

            inst._teamTypes[1] = TeamType.Defense;
            inst._teamDatas[1]._worldPosition = new Vector3(0f, 0f, 16f);
            inst._teamDatas[1]._worldRotation = new Vector3(0f, 180f, 0f);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FileName);
            AssetDatabase.Refresh();
        }

        private void OnEnable() {
            Debug.Log(name + " ScriptableObject OnEnable!");

            if (_teamDatas != null) {
                for (int i = 0; i < DEFAULT_TEAMS_NUM; i++) {
                    TeamData teamData = _teamDatas[i];

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
            }
        }

    }
}
