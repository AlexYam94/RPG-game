using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat{
    public class DamageReceiveComponent : MonoBehaviour, IDamageReceiver{
        //DamageReceiveComponent should put in the part where you want to receive damage
        [SerializeField] float partDamageMultiplier;
        DamageReceiveObject _damageReceiveObject;
        Component _damageController;

        public UnityEvent _onGetHit;
        public UnityEvent onGetHit {get{return _onGetHit;}}

        void Awake() {
            _damageController = GetComponentInParent(typeof(IDamageController));
            _damageReceiveObject = new DamageReceiveObject(_damageController as IDamageController, partDamageMultiplier);
            var rb = GetComponent<Rigidbody>();
            if(rb==null) 
                gameObject.AddComponent<Rigidbody>().isKinematic = true;
            else
                rb.isKinematic = true;
        }

        public void TakeDamage(ICharacter instigator, float damage){
            _damageReceiveObject.TakeDamage(instigator, damage);
        }
    }
}