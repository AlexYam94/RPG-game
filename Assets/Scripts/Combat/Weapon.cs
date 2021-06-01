using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : SerializedMonoBehaviour, IDamageDealer
    {
        [SerializeField] GameObject leftHandGrabPoint = null;
        [SerializeField] ParticleSystem effectOnAttack;
        [SerializeField] TrailRenderer[] trailRenderers;
        [SerializeField] bool canTrigger = false;
        [SerializeField] Dictionary<ArmourType, AudioSource> hitSounds;
        [Range(0,2)][SerializeField] float hitboxMaxEnableTime = .5f;

        private Collider hitbox;
        private float hitboxTimer = 0f;

        public float damage { 
            get => damage; 
            set => damage = value; }

        public ICharacter instigator { 
            get => instigator; 
            set => instigator = value; }

        UnityEvent IDamageDealer.onHit => _onHit;
        UnityEvent IDamageDealer.onAttack => _onAttack;
        UnityEvent IDamageDealer.onStartAttack => _onStartAttack;

        [SerializeField] UnityEvent _onHit = new UnityEvent();
        [SerializeField] UnityEvent _onAttack = new UnityEvent();
        [SerializeField] UnityEvent _onStartAttack = new UnityEvent();


        private void Awake() {
            hitbox = GetComponent<Collider>();
        }

        private void Start() {
            DisableTrail();
        }

        private void Update() {
            if(hitbox.enabled){
                if(hitboxTimer>=hitboxMaxEnableTime)
                    ResetHitbox();
                hitboxTimer+=Time.deltaTime;
            }
        }

        public void OnHit(ArmourType armourType)
        {
            if(hitSounds[armourType]!=null)
                hitSounds[armourType].Play();
            _onHit.Invoke();
        }

        public void OnAttack(){
            _onAttack.Invoke();
        }
        
        public void OnStartAttack(){
            _onStartAttack.Invoke();
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
            hitbox.enabled = true;
            // canTrigger = true;
            // print("EnableTrigger canTrigger: " + canTrigger);
        }

        public void DisableTrigger(){
            // hitbox.enabled = false;
            ResetHitbox();
            // canTrigger = false;
        }

        private void OnTriggerEnter(Collider other) {
            DealDamage(other.GetComponent<IDamageReceiver>());
        }

        private void ResetHitbox(){
            hitbox.enabled = false;
            hitboxTimer = 0f;
        }

        public bool HasEffect(){
            return effectOnAttack == null;
        }

        public bool GetCanTrigger(){
            // return canTrigger;
            return hitbox!=null&&hitbox.enabled;
        }

        public Transform GetGrabObj(){
            return leftHandGrabPoint==null? null: leftHandGrabPoint.transform;
        }

        public void DealDamage(IDamageReceiver target)
        {
            if(target == null) return;
            target.TakeDamage(this.instigator, damage);
        }
    }
}
