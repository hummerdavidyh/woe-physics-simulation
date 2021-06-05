using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public enum EaseType{
        Linear = 0,

        Sine_EaseIn,
        Sine_EaseOut,
        Sine_EaseInOut,


        Quad_EaseIn,
        Quad_EaseOut,
        Quad_EaseInOut,

        Cubic_EaseIn,
        Cubic_EaseOut,
        Cubic_EaseInOut,

        Quart_EaseIn,
        Quart_EaseOut,
        Quart_EaseInOut,

        Quint_EaseIn,
        Quint_EaseOut,
        Quint_EaseInOut,

        Expo_EaseIn,
        Expo_EaseOut,
        Expo_EaseInOut,

        Circ_EaseIn,
        Circ_EaseOut,
        Circ_EaseInOut,

        Elastic_EaseIn,
        Elastic_EaseOut,
        Elastic_EaseInOut,

        Back_EaseIn,
        Back_EaseOut,
        Back_EaseInOut,

        Bounce_EaseIn,
        Bounce_EaseOut,
        Bounce_EaseInOut
    };


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

        public EaseType _easeType = EaseType.Linear;
        /// <summary> </summary>
        public bool _isExportRotation = true;

        /// <summary> </summary>
        public bool _debugAttackTeam = true;
        /// <summary> </summary>
        public bool _debugDefenceTeam = true;

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

        }

        private void FixedUpdate() {
            if (Sample) {

                _curAnimTime += Time.fixedDeltaTime;
                if (_curAnimTime >= _animLength)
                    Sample = false;


                if (FrameNum < 0) {
                    FrameNum = 0;
                }
                else {
                    FrameNum++;
                }

                //_lastTime += Time.deltaTime;
                //if (_lastTime >= 1f / _sampleFrequency) {
                //    FrameNum++;
                //    _lastTime = 0f;
                //}
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
            _lastTime = 0f;
            _curAnimTime = 0f;
        }
        #endregion
    }
}