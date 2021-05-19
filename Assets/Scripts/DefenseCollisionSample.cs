﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {
    public class DefenseCollisionSample : CollsionSample {

        // Start is called before the first frame update
        void Start() {
            Debug.Log("DefenseCollisionSample Start()");

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
                }

                _lastFrameNum = GlobalConfig.Instance.FrameNum;
            }
        }
    }
}
