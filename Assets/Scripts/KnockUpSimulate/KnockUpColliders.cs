using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    [CreateAssetMenu(menuName = "PhysicsSimulation/KnockUpColliders")]
    public class KnockUpColliders : ScriptableObject {

        #region 
        [SerializeField]
        private List<MeshCollider> _meshColliders;
        #endregion

        #region 
        /// <summary> </summary>
        public int Count {
            get {
                return _meshColliders.Count;
            }
        }
        #endregion

        #region 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MeshCollider Get(int id) {
            MeshCollider inst = Instantiate(_meshColliders[id]);
            return inst;
        }
        #endregion
    }
}

