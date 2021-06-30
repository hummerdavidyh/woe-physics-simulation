using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    [CreateAssetMenu(menuName = "GoldLion/Assets/KnockUpDataBase")]
    public class KnockUpDatabase : ScriptableObject {
        #region (Fields)
        [SerializeField]
        private List<KnockUpData> _databases;
        #endregion

        #region 
        /// <summary>  </summary>
        public int Count {
            get {
                return _databases.Count;
            }
        }
        #endregion

        #region (Methods)
        public KnockUpData Get(int id) {
            return _databases[id];
        }
        #endregion
    }
}