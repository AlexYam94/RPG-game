using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit;
        
        [SerializeField] GameObject leftHandGrabPoint = null;

        [SerializeField] ParticleSystem effectOnAttack;

        [SerializeField] TrailRenderer[] trailRenderers;

        [SerializeField] bool canTrigger = false;

        public void OnHit()
        {
            onHit.Invoke();
        }

        public void PlayEffect(){
            if(effectOnAttack!=null)
                effectOnAttack.Play();
        }

        public void EnableTrail(){
            foreach (var trail in trailRenderers)
            {
                if(trail!=null)
                    trail.enabled = true;
            }
        }

        public void DisableTrail(){
            foreach (var trail in trailRenderers)
            {
                if(trail!=null)
                    trail.enabled = false;
            }
        }

        public void EnableTrigger(){
            // GetComponent<BoxCollider>().enabled = true;
            canTrigger = true;
            // print("EnableTrigger canTrigger: " + canTrigger);
        }

        public void DisableTrigger(){
            canTrigger = false;
        }

        // private void OnTriggerEnter(Collider other) {
        //     print("OnTriggerEnter canTrigger: " + canTrigger);
        //     if(!canTrigger){
        //         return;
        //     }
        //     print("trigger");
        //     print(GetComponent<BoxCollider>().enabled);
        //     print(other.gameObject.name);
        //     Health target = other.gameObject.GetComponent<Health>();
        //     if(target == null) return;
        //     float damage = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().GetStat(Stat.Damage);
        //     target.TakeDamage(GameObject.FindGameObjectWithTag("Player"), damage);
        // }

        public bool HasEffect(){
            return effectOnAttack == null;
        }

        public bool GetCanTrigger(){
            return canTrigger;
        }

        public Transform GetGrabObj(){
            return leftHandGrabPoint.transform;
        }
    }
}
