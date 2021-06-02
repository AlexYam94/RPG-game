using UnityEngine;

namespace RPG.Control
{
    public class TargetDetector<T> : MonoBehaviour{
        public bool TargetInRange => _detectedTarget != null;
        private GameObject _detectedTarget; 
        // private string targetTag;

        public Vector3 GetTargetPosition(){
            return _detectedTarget?.transform.position ?? Vector3.zero;
        }

        public GameObject GetTarget(){
            return _detectedTarget;
        }

        private void OnTriggerEnter(Collider other) {
            // if(other.gameObject.tag == targetTag){
            if(other.GetComponent<T>()!=null){
                _detectedTarget = other.transform.gameObject;
            }
        }

        private void OnTriggerExit(Collider other) {
            // if(other.gameObject.tag == targetTag){
            if(other.GetComponent<T>()!=null){
                _detectedTarget = null;
            }
        }
    }
}