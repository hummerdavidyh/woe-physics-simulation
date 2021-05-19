﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {
    public class DefenseCollisionSample : CollsionSample {

        // Start is called before the first frame update
        void Start() {
            Debug.Log("DefenseCollisionSample Start()");

            _teamSampleData = new TeamSampleData();
            _lastFrameNum = GlobalConfig.Instance.FrameNum;

            _children = ArrangeManager.Instance.GetGreenTeams();
            for (int i = 0; i < _children.Count; i++) {
                var child = _children[i];
            }
        }

        // Update is called once per frame
        void Update() {
            if (GlobalConfig.Instance.Sample) {

                if (_lastFrameNum == GlobalConfig.Instance.FrameNum) {
                    return;
                }

                // 录入其它帧的数据
                for (int i = 0; i < _children.Count; i++) {
                    var child = _children[i];

                    Vector3 position = child.localPosition;

                    _teamSampleData.AddPosition((i + 1), GlobalConfig.Instance.FrameNum + GlobalConfig.Instance._interpolationFrame,
                        position.x, position.y, position.z);

                    if (i == 0) {
                        Debug.LogFormat("Defense tag: {0}, position x : {1}, y : {2}, z : {3}, frameIndex: {4}",
                            (i + 1), child.localPosition.x, child.localPosition.y, child.localPosition.z,
                            GlobalConfig.Instance._interpolationFrame + GlobalConfig.Instance.FrameNum);
                    }
                }

                _lastFrameNum = GlobalConfig.Instance.FrameNum;
            }
        }
    }
}