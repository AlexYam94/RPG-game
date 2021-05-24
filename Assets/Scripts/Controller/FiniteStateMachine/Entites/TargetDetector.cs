using UnityEngine;

namespace RPG.Control
{
    public class TargetDetector : MonoBehaviour{
        public bool TargetInRange => _detectedTarget != null;

        private GameObject _detectedTarget;

        public Vector3 GetTargetPosition(){
            return _detectedTarget?.transform.position ?? Vector3.Zero;
        }

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Player"){
                _detectedTarget = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other) {
            if(other.gameObject.tag == "Player"){
                _detectedTarget = null;
            }
        }
    }
}