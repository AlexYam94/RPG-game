using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;
using RPG.Core;

namespace RPG.Combat
{
    public abstract class Projectile : MonoBehaviour, IDamageDealer
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 1;
        [SerializeField] UnityEvent onHit;
        [SerializeField] UnityEvent onLaunch;
        [SerializeField] float heightOffset=1.2f;


        protected Health target = null;
        // protected GameObject instigator = null;

        int layerMask = 0b01100001;
        float damage = 0f;
        bool isLookedAt = false;
        Vector3 shootDirection;
        protected ICharacter instigator;
        ICharacter IDamageDealer.instigator { get => instigator; set => instigator = value; }

        UnityEvent IDamageDealer.onHit => throw new NotImplementedException();
        UnityEvent IDamageDealer.onAttack => throw new NotImplementedException();
        UnityEvent IDamageDealer.onStartAttack => throw new NotImplementedException();
        UnityEvent _onHit = new UnityEvent();
        UnityEvent _onAttack = new UnityEvent();
        UnityEvent _onStartAttack = new UnityEvent();

        float IDamageDealer.damage { get => damage; set => damage = value; }

        private void Start() {
            onLaunch.Invoke();
        }

        // Update is called once per frame
        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit,100f);
            Debug.DrawRay(Camera.main.transform.position, (hit.point-Camera.main.transform.position)*20, Color.green);
            // if (target == null) return;
            // if ((!isLookedAt || isHoming) && !target.GetComponent<Health>().IsDead())
            if(!isLookedAt)
            {
                isLookedAt = true;
                transform.LookAt(GetAimLocation());
            };
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // if(target.GetComponent<Health>().IsDead()&&!isHoming){
            //     Destroy(gameObject,3f);
            //     return;
            // }
            Health health = other.gameObject.GetComponent<Health>();
            // if(target == null) target = health;

            if (instigator.GetHealthComponent()==health || instigator.GetTag()!="Player"&&health != target) return;
            
            // if (instigator.GetHealthComponent()==health) return;
            onHit.Invoke();
            if (hitEffect != null)
            {
                // GameObject.Instantiate(hitEffect, GetAimLocation(), transform.rotation);
                GameObject.Instantiate(hitEffect, transform.position, transform.rotation);
            }
            ApplyDamage(health);
            speed = 0;

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            GetComponent<BoxCollider>().enabled = false;

            Destroy(gameObject, lifeAfterImpact);
        }

        public virtual void ApplyDamage(UnityEngine.Object target){
            if(!typeof(Health).IsAssignableFrom(target.GetType())) return;
            ((Health)target).IsDamageTaken(this.instigator,damage);
            this.target = (Health)target;
            this.SpecialDamage();
        }

        public virtual void SetTarget(Health target, ICharacter instigator, float damage)
        {
            Destroy(gameObject, maxLifeTime);
            this.instigator = instigator;
            this.damage = damage;
            this.target = target;
        }
        
        public virtual void SpecialDamage(){}

        private Vector3 GetAimLocation()
        {
            if(target!=null){
                CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
                if (targetCapsule == null) return target.transform.position;
                return target.transform.position + Vector3.up * targetCapsule.height / 2;
            }else{
                // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // RaycastHit hit;
                // Physics.Raycast(ray, out hit, 30f, layerMask);
                // Vector3 targetPos = hit.point;
                // Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").gameObject.transform.position;
                // targetPos.y = transform.position.y;
                // return targetPos;
                return shootDirection;
            }
            // return Vector3.forward * Time.deltaTime * speed;
        }

        public Health GetTarget(){
            return target;
        }

        public ICharacter GetInstigator() {
            return instigator;
        }


        internal void SetShootDirection(Vector3 shootDirection)
        {
            this.shootDirection = shootDirection;
        }

        public void DealDamage(IDamageReceiver target)
        {
            throw new NotImplementedException();
        }
    }

}