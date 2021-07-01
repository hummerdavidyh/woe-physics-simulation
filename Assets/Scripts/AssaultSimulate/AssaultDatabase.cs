using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    [CreateAssetMenu(menuName = "GoldLion/Assets/AssaultDatabase")]
    public class AssaultDatabase : ScriptableObject {
        #region 
        /// <summary>  </summary>
        [SerializeField]
        private List<AssaultData> _databases;
        #endregion

        #region (Properties)
        public int Count {
            get {
                return _databases.Count;
            }
        }
        #endregion

        #region (Methods)
        public AssaultData Get(int index) {
            return _databases[index];
        }
        #endregion
    }
}
