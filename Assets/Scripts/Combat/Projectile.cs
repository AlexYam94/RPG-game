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
        [SerializeField] float heightOffset=1.2f;

        int layerMask = 0b00000001;
        Health target = null;
        GameObject instigator = null;
        float damage = 0f;
        bool isLookedAt = false;
        Vector3 shootDirection;
        [SerializeField] float testNum;

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

            if (instigator.GetComponent<Health>()==health || instigator.gameObject.tag!="Player"&&health != target) return;

            onHit.Invoke();
            if (hitEffect != null)
            {
                // GameObject.Instantiate(hitEffect, GetAimLocation(), transform.rotation);
                GameObject.Instantiate(hitEffect, transform.position, transform.rotation);
            }
            health.TakeDamage(this.instigator,damage);

            speed = 0;

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            GetComponent<BoxCollider>().enabled = false;

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

        public GameObject GetInstigator() {
            return instigator;
        }

        internal void SetShootDirection(Vector3 shootDirection)
        {
            shootDirection.y = transform.position.y/testNum;
            this.shootDirection = shootDirection;
        }
    }

}