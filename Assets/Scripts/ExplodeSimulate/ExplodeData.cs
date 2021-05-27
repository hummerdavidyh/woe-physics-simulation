using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {
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
        public FireballData _fireballData;

        [SerializeField]
        public TeamData _teamData;
        #endregion


        [MenuItem("GoldLion/Assets/ExplodeData")]
        static void CreateAssetInstance() {
            if (!Directory.Exists(ASSET_PATH)) {
                Directory.CreateDirectory(ASSET_PATH);
            }

            ExplodeData inst = CreateInstance<ExplodeData>();
            string FileName = string.Format("{0}/{1}.asset", ASSET_PATH, ASSET_NAME);
            AssetDatabase.CreateAsset(inst, FileName);

            // Fireball data.
            FireballData fireball = CreateInstance<FireballData>();
            fireball._worldPosition = new Vector3(0f, 0f, -50f);
            fireball._worldRotation = new Vector3(0f, 0f, 0f);
            fireball._attackDirection = new Vector3(0f, 1f, 1f);
            fireball._attackSpeed = 50f; 
            fireball._collider = new ColliderData {
                _centre = new Vector3(0f, 0.25f, 0f),
                _size = new Vector3(0.5f, 0.5f, 0.5f)
            };
            fireball._rigid = new RigidData {
                _mass = 15f,
                _drag = 0.5f
            };

            // Team data.
            TeamData teamData = CreateInstance<TeamData>();
            teamData.name = "DefenceTeam";
            teamData._total = DEFAULT_TEAM_TOTAL;
            teamData._row = DEFAULT_TEAM_ROW_NUM;
            teamData._spanRow = DEFAULT_TEAM_SPAW_ROW;
            teamData._spanColumn = DEFAULT_TEAM_SPAW_COLUMN;

            teamData._colliders = new List<ColliderData>();
            for (int k = 0; k < teamData._row; k++) {
                var data = new ColliderData {
                    _centre = new Vector3(0f, 0.25f, 0f),
                    _size = new Vector3(0.5f, 0.5f, 0.5f)
                };

                teamData._colliders.Add(data);
            }

            teamData._rigids = new List<RigidData>();
            for (int k = 0; k < teamData._row; k++) {
                var data = new RigidData {
                    _mass = 15f,
                    _drag = 0.5f
                };

                teamData._rigids.Add(data);
            }
            teamData._worldPosition = new Vector3(0f, 0f, 0f);
            teamData._worldRotation = new Vector3(0f, 0f, 0f);
            inst._teamData = teamData;
            AssetDatabase.AddObjectToAsset(inst._teamData, FileName);
            
            // update assets.
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FileName);
            AssetDatabase.Refresh();
        }
    }
}