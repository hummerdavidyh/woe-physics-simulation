using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {
    public class DefenseCollisionSample : CollsionSample {

        // Start is called before the first frame update
        void Start() {
            Debug.Log("AssaultDefenseCollisionSample Start()");

            _teamSampleData = new TeamExportData();
            _lastFrameNum = GlobalConfig.Instance.FrameNum;

            if (_children == null) {
                _children = new List<Transform>();
            }
            var rigidBodys = GetComponentsInChildren<Rigidbody>();
            foreach (var rigidBody in rigidBodys) {
                _children.Add(rigidBody.gameObject.transform);
            }
        }

        // Update is called once per frame
        void Update() {
            if (GlobalConfig.Instance.Sample) {

                if (_lastFrameNum == GlobalConfig.Instance.FrameNum) {
                    return;
                }

                if (GlobalConfig.Instance.FrameNum == 0)
                {
                    // 添加标识帧数据
                    for (int i = 0; i < _children.Count; i++)
                    {
                        var child = _children[i];

                        _teamSampleData.AddPosition((i + 1), 1, 0f, 0f, 0f, 1000f);

                        Quaternion quat = new Quaternion(0f, 0f, 0f, 999f);
                        _teamSampleData.AddQuaternion((i + 1), 1, quat);
                    }
                }


                // 录入其它帧的数据
                for (int i = 0; i < _children.Count; i++) {


                    var child = _children[i];

                    Vector3 position = child.localPosition;
                    position.x *= -1f;

                    _teamSampleData.AddPositionForCocos((i + 1), GlobalConfig.Instance.FrameNum + GlobalConfig.Instance._interpolationFrame,
                        position.x, position.y, position.z);

                    if (GlobalConfig.Instance._isExportRotation)
                    {
                        _teamSampleData.AddQuaternion((i + 1), GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum,
                            child.rotation);
                    }

                    if (i == 0)
                    {
                        PrintDebugInfo("Defense tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                            (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                            GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum);
                    }
                
                }

                _lastFrameNum = GlobalConfig.Instance.FrameNum;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private void PrintDebugInfo(string format, params object[] args) {

            if (!GlobalConfig.Instance._debugDefenceTeam) {
                return;
            }

            Debug.LogFormat(format, args);
        }
    }
}
