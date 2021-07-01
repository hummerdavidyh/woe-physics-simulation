using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace GoldenLion.PhysicsSimulation {

    public class CollsionSample : MonoBehaviour {
        /// <summary> 队列采样数据 </summary>
        protected TeamExportData _teamSampleData;
        /// <summary> 队列包含的对象 </summary>
        protected List<Transform> _children;

        protected int _lastFrameNum;

        // Start is called before the first frame update
        void Start() {
            Debug.Log("CollsionSample Start()");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void SaveToFile(string fileName) {
            _teamSampleData.SetDuration(GlobalConfig.Instance.FrameNum + GlobalConfig.Instance._interpolationFrame);
            _teamSampleData.SetSpeed(1);
            string jsonContent = JsonUtility.ToJson(_teamSampleData);

            if (!File.Exists(fileName)) {
                File.WriteAllText(fileName, jsonContent);
            }
        }
    }
}
