using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenLion.PhysicsSimulation {

    public class CubeTrigger : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnTriggerEnter(Collider other) {
            Debug.Log("OnTriggerEnter");
            gameObject.SetActive(false);

            GlobalConfig.Instance.Sample = true;           
        }
    }
}
