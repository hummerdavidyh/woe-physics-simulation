using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {
    public enum FrameType {
        Position,
        Position3D,
        Rotation3D,
        Quaternion,
    }

    /// <summary>
    /// 变换数据
    /// </summary>
    [Serializable]
    public class TransData {
        [SerializeField]
        public float x;
        [SerializeField]
        public float y;
        [SerializeField]
        public float z;
        [SerializeField]
        public float w;

        [SerializeField]
        public int frameIndex;
    }

    /// <summary>
    /// 变换曲线
    /// </summary>
    [Serializable]
    public class TransCurve {
        #region Variables
        /// <summary>  </summary>
        [SerializeField]
        public int actionTag;
        /// <summary>  </summary>
        [SerializeField]

        public string frameType;
        /// <summary>  </summary>
        [SerializeField]
        public List<TransData> frames;
        #endregion

        #region (Constructors)
        public TransCurve() {
            frames = new List<TransData>();
        }
        #endregion

        #region (Methods)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        public TransData Find(int frameIndex) {
            for (int i = 0; i < frames.Count; i++) {
                if (frames[i].frameIndex == frameIndex)
                    return frames[i];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        public bool Exist(int frameIndex) {
            return Find(frameIndex) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public void Add(int frameIndex, float x, float y, float z, float w) {
            TransData data = new TransData();

            data.frameIndex = frameIndex;
            data.x = x;
            data.y = y;
            data.z = z;
            data.w = w;

            frames.Add(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            frames.Clear();
        }
        #endregion
    }
    
    /// <summary>
    /// 队列采样数据
    /// </summary>
    [Serializable]
    public class TransAnimation {
        [SerializeField]
        public int duration;
        [SerializeField]
        public int speed;

        [SerializeField]
        public List<TransCurve> timelines;

        public TransAnimation() {
            timelines = new List<TransCurve>();
        }

        #region (Methods)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="frameType"></param>
        /// <returns></returns>
        public TransCurve Find(int tag, FrameType frameType) {
            for (int i = 0; i < timelines.Count; i++) {
                if (timelines[i].actionTag == tag
                 && timelines[i].frameType == frameType.ToString()) {

                    return timelines[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="frameType"></param>
        /// <returns></returns>
        public bool Exist(int tag, FrameType frameType) {
            return Find(tag, frameType) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="frameType"></param>
        /// <param name="frameIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public void Add(int tag, FrameType frameType, int frameIndex, float x, float y, float z, float w) {
            if (Exist(tag, frameType)) {
                TransCurve curve = Find(tag, frameType);

                if (curve.Exist(frameIndex)) {
                    throw new Exception(string.Format(
                        "tag = {0} frameType = {1} frameIndex = {2}", tag, frameType, frameIndex));
                }
                else {
                    curve.Add(frameIndex, x, y, z, w);
                }
            }
            else {
                TransCurve curve = new TransCurve();
                curve.actionTag = tag;
                curve.frameType = frameType.ToString();
                curve.Add(frameIndex, x, y, z, w);

                timelines.Add(curve);
            }
        }
        #endregion

    }

    [Serializable]
    public class TeamSampleData {
        private const float UNITY2COCOS_POSITION_SCALE = 100f;

        #region 
        [SerializeField]
        private TransAnimation action;
        #endregion

        #region (Methods)
        /// <summary>
        /// 
        /// </summary>
        public TeamSampleData() {
            action = new TransAnimation();
        }
        #endregion

        #region 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="frameIndex"></param>
        /// <param name="position"></param>
        public void AddPosition(int tag, int frameIndex, float x, float y, float z) {
            float cocosX = x * UNITY2COCOS_POSITION_SCALE;
            float cocosY = y * UNITY2COCOS_POSITION_SCALE;
            float cocosZ = z * UNITY2COCOS_POSITION_SCALE;
            action.Add(tag, FrameType.Position3D, frameIndex, cocosX, cocosY, cocosZ, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="frameIndex"></param>
        /// <param name="quat"></param>
        public void AddQuaternion(int tag, int frameIndex, Quaternion quat) {
            action.Add(tag, FrameType.Quaternion, frameIndex, quat.x, quat.y, quat.z, quat.w);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        public void SetDuration(int duration) {
            action.duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(int speed) {
            action.speed = speed;
        }
        #endregion
    }
}