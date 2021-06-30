using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class AssaultAttackCollisionSample : CollsionSample {

        #region (Variables) Private
        /// <summary> </summary>
        private List<Vector3> _originalPositions;
        /// <summary> </summary>
        private List<Vector3> _startFramePositions;
        /// <summary> </summary>
        private List<Vector3> _differentPositions;        
        #endregion

        #region (Methods) Unity
        // Start is called before the first frame update
        void Start() {            
            Debug.Log("AssaultAttackCollisionSample Start()");

            _teamSampleData = new TeamSampleData();
            _lastFrameNum = GlobalConfig.Instance.FrameNum;

            _originalPositions = new List<Vector3>();
            _startFramePositions = new List<Vector3>();
            _differentPositions = new List<Vector3>();

            if (_children == null) {
                _children = new List<Transform>();
            }
            var rigidBodys = GetComponentsInChildren<Rigidbody>();
            foreach (var rigidBody in rigidBodys) {
                _children.Add(rigidBody.gameObject.transform);
            }
            
            for (int i = 0; i < _children.Count; i++) {
                var child = _children[i];

                _originalPositions.Add(child.localPosition);

                PrintDebugInfo("Attack tag: {0}, original Position x : {1}, y : {2}, z : {3}",
                  (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z);
            }
        }

        // Update is called once per frame
        void Update() {
            if (GlobalConfig.Instance.Sample) {

                if (_lastFrameNum == GlobalConfig.Instance.FrameNum) {
                    return;
                }

                if (GlobalConfig.Instance.FrameNum == 0) {
                    // 获得起始帧的各个孩子的位置（相对于阵型中心点的位置）
                      for (int i = 0; i < _children.Count; i++) {
                        var child = _children[i];

                        _startFramePositions.Add(child.localPosition);

                        PrintDebugInfo("Attack tag: {0}, startFrame Position x : {1}, y : {2}, z : {3}",
                            (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z);
                    }

                    // 做差值
                    for (int i = 0; i < _children.Count; i++) {
                        Vector3 diff = _startFramePositions[i] - _originalPositions[i];
                        _differentPositions.Add(diff);
                    }

                    for (int i = 0; i < _children.Count; i++)
                    {
                        var child = _children[i];

                        // 添加标识帧数据
                        _teamSampleData.AddPosition((i + 1), 1, 0f, 0f, 0f, 1000f + (int)GlobalConfig.Instance._easeType);

                        Quaternion quat = new Quaternion(0f, 0f, 0f, 999f);
                        _teamSampleData.AddQuaternion((i + 1), 1, quat);
                    }

                        // 录入第一帧的数据
                    for (int i = 0; i < _children.Count; i++) {
                        var child = _children[i];

                        // 添加标识帧数据
                        Vector3 position = child.localPosition - _differentPositions[i];
                        if (i == 0 && GlobalConfig.Instance._debugAttackTeam)
                        {
                            PrintDebugInfo("Attack tag: {0}, child localPosition x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                                (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                                GlobalConfig.Instance._interpolationFrame);

                            PrintDebugInfo("Attack tag: {0}, differentPositions x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                                (i + 1), _differentPositions[i].x, _differentPositions[i].y, _differentPositions[i].z,
                                GlobalConfig.Instance._interpolationFrame);

                            PrintDebugInfo("Attack tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                                (i + 1), position.x, position.y, position.z,
                                GlobalConfig.Instance._interpolationFrame);
                        }

                        _teamSampleData.AddPositionForCocos((i + 1), GlobalConfig.Instance._interpolationFrame,
                            position.x, position.y, position.z);

                        if (GlobalConfig.Instance._isExportRotation)
                        {
                            _teamSampleData.AddQuaternion((i + 1), GlobalConfig.Instance._interpolationFrame,
                                child.rotation);
                        }
                        
                        //if (i == 0) {
                        //    Debug.LogFormat("Attack tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                        //        (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                        //        GlobalConfig.Instance._interpolationFrame);
                        //}
                    }
                }
                else {
                    // 录入后续帧的数据
                    for (int i = 0; i < _children.Count; i++) {
                        var child = _children[i];

                        Vector3 position = child.localPosition - _differentPositions[i];

                        _teamSampleData.AddPositionForCocos((i + 1), GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum,
                            position.x, position.y, position.z);

                        if (GlobalConfig.Instance._isExportRotation)
                        {
                            _teamSampleData.AddQuaternion((i + 1), GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum,
                                child.rotation);
                        }

                        if (i == 0 ) {
                            PrintDebugInfo("Attack tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                                (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                                GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum);
                        }
                    }
                }

                _lastFrameNum = GlobalConfig.Instance.FrameNum;
            }
        }
        #endregion


        private void PrintDebugInfo(string format, params object[] args) {

            if (!GlobalConfig.Instance._debugAttackTeam) {
                return;
            }

            Debug.LogFormat(format, args);
        }
    }
}