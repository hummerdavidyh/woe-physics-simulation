using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    [Serializable]
    public class FireballData : ScriptableObject {
        [SerializeField]
        public bool _foldOut;

        [SerializeField]
        public Vector3 _worldPosition;

        [SerializeField]
        public Vector3 _worldRotation;

        [SerializeField]
        public Vector3 _attackDirection;

        [SerializeField]
        public float _attackSpeed;

        [SerializeField]
        public ColliderData _collider;

        [SerializeField]
        public RigidData _rigid;
    }
}