using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    [Serializable]
    public class ColliderData {
        [SerializeField]
        public Vector3 _centre;
        [SerializeField]
        public Vector3 _size;
        [SerializeField]
        public bool _foldOut;
    }

    [Serializable]
    public class RigidData {
        [SerializeField]
        public float _mass;
        [SerializeField]
        public float _drag;
    }

    [Serializable]
    public class TeamData : ScriptableObject{
        [SerializeField]
        public Vector3 _worldPosition;

        [SerializeField]
        public Vector3 _worldRotation;

        [SerializeField]
        public int _total;

        [SerializeField]
        public int _row;

        [SerializeField]
        public float _spanRow;

        [SerializeField]
        public float _spanColumn;

        [SerializeField]
        public List<ColliderData> _colliders;

        [SerializeField]
        public List<RigidData> _rigids;

        [SerializeField]
        public bool _writeBack;

        [SerializeField]
        public List<Vector3> _localPositions;

        [SerializeField]
        public List<Quaternion> _localRotations;
    }
}


