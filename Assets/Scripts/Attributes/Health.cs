using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using RPG.Combat;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;
using RPG.Control;
using RPG.Tool;
using Sirenix.OdinInspector;

namespace RPG.Attributes
{
    public class Health : SerializedMonoBehaviour, ISaveable 
    {
        [SerializeField][Range(0, 100)] float healthRegenerationOnLevelUp = 10f;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;
        [SerializeField] float blockingMultiplier = .5f;
        [SerializeField] float damageTakenCooldownTime = .5f;
        [SerializeField][Range(0, 100)] float healthRegen = 100f;
        public Dictionary<GameObject, float> damageTakenCooldown = null;
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
            damageTakenCooldown = new Dictionary<GameObject, float>();
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
            List<GameObject> keys = new List<GameObject>(damageTakenCooldown.Keys);
            foreach(var key in keys){
                Debug.Log(damageTakenCooldown[key]);
            }
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
            List<GameObject> keys = new List<GameObject>(damageTakenCooldown.Keys);
            foreach(var key in keys){
                damageTakenCooldown[key] = Mathf.Max(damageTakenCooldown[key] - Time.deltaTime,0);
            }
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isDead) return;
            instigator.GetComponent<Health>().AddTargetToNotify(gameObject);
            lock(damageTakenCooldown){
                if(!damageTakenCooldown.ContainsKey(instigator)){
                    damageTakenCooldown.Add(instigator, damageTakenCooldownTime);
                }
                if(damageTakenCooldown[instigator]>0){
                    return;
                }else{
                    damageTakenCooldown[instigator] = damageTakenCooldownTime;
                    if(gameObject.tag == "Player"){
                        if(isInvulnerable){
                            return;
                        }else if(gameObject.GetComponent<PlayerController>().IsBlocking()){
                            Vector3 direction = instigator.transform.position - transform.position;
                            float angle = Vector3.Angle(direction, transform.forward);
                            if(angle <= Util.GetBlockingAngle(gameObject)/2){
                                damage *= blockingMultiplier;
                            }
                        }
                    }
                    healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
                    CheckDeath(instigator);

                    takeDamage.Invoke(damage);
                }
            }
        }

        private void CheckDeath(GameObject instigator)
        {
            if (healthPoints.value <= 0)
            {
                Experience exp = instigator.GetComponent<Experience>();
                if (exp != null)
                {
                    exp.GainExperince(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
                onDie.Invoke();
                Die();
            }
        }

        public void StartDamageOfTime(float damage, float duration, GameObject instigator){
            damageOfTimeCounter = duration;
            damagePerSec = damage/duration;
            StartCoroutine(DamageOfTime(damagePerSec,instigator));
        }

        public IEnumerator DamageOfTime(float damagePerSecond, GameObject instigator){
            yield return new WaitForSeconds(0.5f);
            while (damageOfTimeCounter > 0)
            {
                TakeDamage(instigator, damagePerSecond);
                damageOfTimeCounter -= 1;
                CheckDeath(instigator);
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

        private void Die()
        {
            if(isDead) return;
                isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
            Health[] objects = UnityEngine.Object.FindObjectsOfType<Health>();
            // for (var i = 0; i < objects.Length; i++)
            // {
            //     if(objects[i].gameObject == gameObject)
            //         continue;
            //     objects[i].RemoveInstigator(gameObject);
            // }
            foreach (var item in targetsToNotify)
            {
                item.GetComponent<Health>().RemoveInstigator(gameObject);
            }
        }

        public void SetInvulnerable(bool target){
            isInvulnerable = target;
        }

        public float GetInstigatorDamageCooldown(GameObject key){
            if(!damageTakenCooldown.ContainsKey(key))
                return 0;
            return damageTakenCooldown[key];
        }

        public void RemoveInstigator(GameObject key){
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
    }
}

