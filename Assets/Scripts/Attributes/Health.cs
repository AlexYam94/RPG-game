﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.Core;
using RPG.Combat;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;
using RPG.Control;
using RPG.Tool;
using Sirenix.OdinInspector;
using GameDevTV.Saving;

namespace RPG.Attributes
{
    public class Health : SerializedMonoBehaviour, ISaveable, IAttribute, IHealth
    {
        [SerializeField][Range(0, 100)] float healthRegenerationOnLevelUp = 10f;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;
        [SerializeField] float blockingMultiplier = .5f;
        [SerializeField] float damageTakenCooldownTime = .5f;
        [SerializeField][Range(0, 100)] float healthRegen = 0f;
        [SerializeField] public List<GameObject> gameObjectToDisable = new List<GameObject>();
        public Dictionary<ICharacter, float> damageTakenCooldown = null;
        public List<GameObject> targetsToNotify = null;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>{};

        LazyValue<float> healthPoints;
        bool isDead;
        float currentPercentage = 1f;
        float damageOfTimeCounter = 0;
        float damagePerSec = 0;
        private bool isInvulnerable = false;



        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            GetComponent<BaseStats>().onLevelUp += UpdateHealth;
            damageTakenCooldown = new Dictionary<ICharacter, float>();
            targetsToNotify = new List<GameObject>();
        }

        private float GetInitialHealth(){
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        // Start is called before the first frame update
        void Start()
        {
            healthPoints.ForceInit();
        }

        private void Update() {
            currentPercentage = GetFraction();
            UpdateDamageTakenCooldown();
            Heal(healthRegen);
        }

        private void UpdateHealth()
        {
            float nextHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            float percentage = Mathf.Clamp(currentPercentage,0,100);
            float tempHealth = nextHealth * (percentage + healthRegenerationOnLevelUp/100);
            // float tempHealth = GetComponent<BaseStats>().GetStat(Stat.Health) * (Mathf.Clamp(currentPercentage,0,100) + healthRegenerationOnLevelUp)/100;
            healthPoints.value = Mathf.Clamp(tempHealth,tempHealth,nextHealth);
        }

        private void UpdateDamageTakenCooldown(){
            List<ICharacter> keys = new List<ICharacter>(damageTakenCooldown.Keys);
            foreach(var key in keys){
                damageTakenCooldown[key] = Mathf.Max(damageTakenCooldown[key] - Time.deltaTime,0);
            }
        }

        public bool IsDead(){
            return isDead;
        }

        public bool IsDamageTaken(ICharacter instigator, float damage)
        {
            if (isDead) return false;
            
            instigator.GetHealthComponent().AddTargetToNotify(gameObject);
            lock(damageTakenCooldown){
                if(!damageTakenCooldown.ContainsKey(instigator)){
                    damageTakenCooldown.Add(instigator, 0);
                }
                if(damageTakenCooldown[instigator]>0){
                    return false;
                }else{
                    damageTakenCooldown[instigator] = damageTakenCooldownTime;
                    if(gameObject.tag == "Player"){
                        if(isInvulnerable){
                            return false;
                        }else if(gameObject.GetComponent<PlayerController>().IsBlocking()){
                            Vector3 direction = instigator.GetGameObject().transform.position - transform.position;
                            float angle = Vector3.Angle(direction, transform.forward);
                            if(angle <= Util.GetBlockingAngle(gameObject)/2){
                                damage *= blockingMultiplier;
                            }
                        }
                    }
                    healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
                    CheckDeath(instigator.GetGameObject());
                    if(!isDead&&(gameObject.name.ToLower().Contains("grunt")||gameObject.name.ToLower().Contains("archer"))){
                        GetComponent<Animator>().SetTrigger("recoiling");
                    }

                    takeDamage.Invoke(damage);
                }
                return true;
            }
        }

        private void CheckDeath(GameObject instigator)
        {
            if (healthPoints.value <= 0)
            {
                Experience exp = instigator.GetComponent<Experience>();
                if (exp != null)
                {
                    exp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
                onDie.Invoke();
                Die();
            }
        }

        public void StartDamageOfTime(float damage, float duration, ICharacter instigator){
            damageOfTimeCounter = duration;
            damagePerSec = damage/duration;
            StartCoroutine(DamageOfTime(damagePerSec,instigator));
        }

        public IEnumerator DamageOfTime(float damagePerSecond, ICharacter instigator){
            yield return new WaitForSeconds(0.5f);
            while (damageOfTimeCounter > 0)
            {
                IsDamageTaken(instigator, damagePerSecond);
                damageOfTimeCounter -= 1;
                CheckDeath(instigator.GetGameObject());
                yield return new WaitForSeconds(1f);
            }
        }

        public float GetHealthPoints(){
            return healthPoints.value;
        }

        public float GetMaxHealthPoints(){
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage(){
            return healthPoints.value/GetComponent<BaseStats>().GetStat(Stat.Health) * 100;
        }

        public float GetFraction(){
            return healthPoints.value/GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void Die()
        {
            if(isDead) return;
                isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            Collider[] colliders = GetComponents<Collider>();
            GetComponent<Stamina>().enabled = false;
            foreach (var item in targetsToNotify)
            {
                item.GetComponent<Health>().RemoveInstigator(gameObject.GetComponent<ICharacter>());
            }
            foreach (GameObject obj in gameObjectToDisable)
            {
                obj.SetActive(false);
            }
            foreach (var item in colliders)
            {   
                item.enabled = false;
            }
        }

        public void SetInvulnerable(bool target){
            isInvulnerable = target;
        }

        public float GetInstigatorDamageCooldown(ICharacter key){
            if(!damageTakenCooldown.ContainsKey(key))
                return 0;
            return damageTakenCooldown[key];
        }

        public void RemoveInstigator(ICharacter key){
            damageTakenCooldown.Remove(key);
        }


        public void AddTargetToNotify(GameObject target){
            if(!targetsToNotify.Contains(target)){
                targetsToNotify.Add(target);
            }
        }

        public void RemoveTargetToNotify(GameObject target){
            targetsToNotify.Remove(target);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if(healthPoints.value <= 0) Die();
        }

        public bool isEnable()
        {
            return GetComponent<Health>().enabled;
        }

        public void ApplyDamage(float damage)
        {
            takeDamage.Invoke(damage);
        }
    }
}

