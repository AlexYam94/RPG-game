using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Combat{
    public class DamageController : MonoBehaviour, IDamageController{
        [TabGroup("DamageReceiverAttributes")]
        [SerializeField] float blockingMultiplier = .5f;
        Dictionary<ICharacter, float> damageTakenCooldown = null;
        // List<GameObject> targetsToNotify;
        bool _isBlocking = false;
        bool _isInvulnerable = false;
        IHealth _health;

        void Awake(){
           damageTakenCooldown = new Dictionary<ICharacter, float>();
        //    targetsToNotify = new List<GameObject>(); 
           _health = GetComponent(typeof(IHealth)) as IHealth;
        }

        public bool CanTakeDamageFromGambObject(ICharacter instigator){
            // instigator.GetComponent<DamageController>().AddTargetToNotify(gameObject);
            lock(damageTakenCooldown){
                if(!damageTakenCooldown.ContainsKey(instigator)){
                    damageTakenCooldown.Add(instigator, 0);
                }
                if(damageTakenCooldown.ContainsKey(instigator)&&damageTakenCooldown[instigator]>0){
                    return false;
                }else{
                    damageTakenCooldown.Remove(instigator);
                    if(gameObject.tag == "Player"){
                        if(_isInvulnerable){
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        private float GetBlockingDamage(ICharacter instigator, float damage)
        {       
                if(!GetIsBlocking()) return damage;
                Vector3 direction = instigator.GetGameObject().transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);
                if (angle <= Util.GetBlockingAngle(gameObject) / 2)
                {
                    damage *= blockingMultiplier;
                }
                return damage;
        }


        private void UpdateDamageTakenCooldown(){
            // foreach(KeyValuePair<ICharacter, float> entry in damageTakenCooldown){
            //     entry.value = Mathf.Max(damageTakenCooldown[key] - Time.deltaTime,0);
            // }
            foreach(var key in damageTakenCooldown.Keys){
                damageTakenCooldown[key] = Mathf.Max(damageTakenCooldown[key] - Time.deltaTime,0);
            }

        }

        // private void AddTargetToNotify(GameObject target){
        //     targetsToNotify.Add(target);
        // }

        // public void RemoveTargetToNotify(GameObject target){
        //     targetsToNotify.Remove(target);
        // }
        public void ApplyDamage(ICharacter instigator, float damage){
            _health.ApplyDamage(GetBlockingDamage(instigator, damage));
        }

        public bool GetIsBlocking(){
            return _isBlocking;
        }

        public void SetIsBlocking(bool isBlocking){
            _isBlocking = isBlocking;
        }

        public void SetInvulnerable(bool isInvulnerable){
            _isInvulnerable = isInvulnerable;
        }
    }
}