﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class AttackCollisionSample : CollsionSample {

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
            Debug.Log("AttackCollisionSample Start()");

            _teamSampleData = new TeamSampleData();
            _lastFrameNum = GlobalConfig.Instance.FrameNum;

            _originalPositions = new List<Vector3>();
            _startFramePositions = new List<Vector3>();
            _differentPositions = new List<Vector3>();

            _children = ArrangeManager.Instance.GetYellowTeams();
            for (int i = 0; i < _children.Count; i++) {
                var child = _children[i];

                _originalPositions.Add(child.localPosition);
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
                        _teamSampleData.AddPosition((i + 1), 1, 0f, 0f, 999.0f);
                    }

                        // 录入第一帧的数据
                    for (int i = 0; i < _children.Count; i++) {
                        var child = _children[i];

                        // 添加标识帧数据
                       
                        _teamSampleData.AddPositionForCocos((i + 1), GlobalConfig.Instance._interpolationFrame,
                            child.localPosition.x, child.localPosition.y, child.localPosition.z);
                        _teamSampleData.AddQuaternion((i + 1), GlobalConfig.Instance._interpolationFrame,
                            child.rotation);

                        if (i == 0) {
                            Debug.LogFormat("Attack tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                                (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                                GlobalConfig.Instance._interpolationFrame);
                        }
                    }
                }
                else {
                    // 录入后续帧的数据
                    for (int i = 0; i < _children.Count; i++) {
                        var child = _children[i];

                        Vector3 position = child.localPosition - _differentPositions[i];

                        _teamSampleData.AddPosition((i + 1), GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum,
                            position.x, position.y, position.z);
                        _teamSampleData.AddQuaternion((i + 1), GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum,
                            child.rotation);

                        if (i == 0) {
                            Debug.LogFormat("Attack tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                                (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                                GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum);
                        }
                    }
                }

                _lastFrameNum = GlobalConfig.Instance.FrameNum;
            }
        }
        #endregion
    }
}