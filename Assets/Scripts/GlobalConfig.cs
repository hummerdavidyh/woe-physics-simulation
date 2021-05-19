using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class GlobalConfig : MonoBehaviourSingleton<GlobalConfig> {
        #region (Varibles) Consts
        /// <summary> </summary>
        public const int INVALID_FRAME_NUM = -1;
        #endregion

        #region (Variables) Public
        /// <summary> </summary>
        [Range(30, 60)]
        public float _sampleFrequency = 30f;
        /// <summary> </summary>
        [Range(0.1f, 5f)]
        public float _animLength = 1f;
        /// <summary> </summary>
        [Range(1, 200)]
        public int _interpolationFrame = 50;
        #endregion

        #region (Variables) Private
        /// <summary> </summary>
        private float _lastTime = 0f;
        /// <summary> </summary>
        private float _curAnimTime = 0f;
        #endregion

        #region (Properties)
        /// <summary>
        /// 
        /// </summary>
        public bool Sample {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int FrameNum {
            get;
            private set;
        }

        #endregion

        #region (Methods) Unity
        protected override void SingletonStarted() {
            base.SingletonStarted();

            Clear();
        }

        private void Update() {
            if (Sample) {

                _curAnimTime += Time.deltaTime;
                if (_curAnimTime >= _animLength)
                    Sample = false;


                if (FrameNum < 0) {
                    FrameNum = 0;
                }

                _lastTime += Time.deltaTime;
                if (_lastTime >= 1f / _sampleFrequency) {
                    FrameNum++;
                    _lastTime = 0f;
                }
             }
        }
        #endregion

        #region (Methods)
        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            Sample = false;
            FrameNum = INVALID_FRAME_NUM;
            _lastTime = 0;
            _curAnimTime = 0;
        }
        #endregion
    }
}