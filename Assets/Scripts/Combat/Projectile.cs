using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 1;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0f;
        bool isLookedAt = false;

        // Update is called once per frame
        private void Update()
        {
            if (target == null) return;
            if ((!isLookedAt || isHoming) && !target.GetComponent<Health>().IsDead())
            {
                isLookedAt = true;
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // if(target.GetComponent<Health>().IsDead()&&!isHoming){
            //     Destroy(gameObject,3f);
            //     return;
            // }
            Health health = other.gameObject.GetComponent<Health>();

            if (health != target) return;
            print(target.gameObject.name);

            onHit.Invoke();
            if (hitEffect != null)
            {
                GameObject.Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            target.TakeDamage(this.instigator,damage);

            speed = 0;

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            Destroy(gameObject, maxLifeTime);
            this.instigator = instigator;
            this.damage = damage;
            this.target = target;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
            // return Vector3.forward * Time.deltaTime * speed;
        }

        public Health GetTarget(){
            return target;
        }

        public GameObject GetInstigator() {
            return instigator;
        }
    }

}