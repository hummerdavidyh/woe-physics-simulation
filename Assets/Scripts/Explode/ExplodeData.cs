using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    [Serializable]
    public class ExplodeData : ScriptableObject {
        #region (Variables) consts
        [NonSerialized]
        public const int DEFAULT_TEAM_TOTAL = 50;
        [NonSerialized]
        public const int DEFAULT_TEAM_ROW_NUM = 5;
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_ROW = 1f;
        [NonSerialized]
        public const float DEFAULT_TEAM_SPAW_COLUMN = 1f;
        [NonSerialized]
        public const string ASSET_PATH = "Assets/Resources/DatabaseExplode";
        [NonSerialized]
        public const string ASSET_NAME = "defaultExplode";
        #endregion

        #region (Variables) 
        [SerializeField]
        public TeamData _teamData;
        [SerializeField]
        public List<bool> _foldouts;
        #endregion


        [MenuItem("GoldLion/Assets/ExplodeData")]
        static void CreateAssetInstance() {
            if (!Directory.Exists(ASSET_PATH)) {
                Directory.CreateDirectory(ASSET_PATH);
            }

            ExplodeData inst = CreateInstance<ExplodeData>();
            string FileName = string.Format("{0}/{1}.asset", ASSET_PATH, ASSET_NAME);
            AssetDatabase.CreateAsset(inst, FileName);

            inst._teamData = new TeamData();
            inst._foldouts = new List<bool>();
            inst._foldouts.Add(false);

            var teamData = CreateInstance<TeamData>();
            teamData.name = "DefenceTeam";
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

            inst._teamData._worldPosition = new Vector3(0f, 0f, 0f);
            inst._teamData._worldRotation = new Vector3(0f, 0f, 0f);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FileName);
            AssetDatabase.Refresh();
        }
    }
}