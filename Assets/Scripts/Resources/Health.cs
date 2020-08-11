using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using RPG.Combat;
using System;
using GameDevTV.Utils;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField][Range(0, 100)] float healthRegenerationOnLevelUp = 1f;

        LazyValue<float> healthPoints;
        bool isDead;
        float currentPercentage = -1f;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth(){
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        // Start is called before the first frame update
        void Start()
        {
            healthPoints.ForceInit();
        }



        private void updateHealth()
        {
            float tempHealth = GetComponent<BaseStats>().GetStat(Stat.Health) * (Mathf.Clamp(currentPercentage,0,100) + healthRegenerationOnLevelUp)/100;
            healthPoints.value = tempHealth;
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isDead) return;
            print(gameObject.name + " took damage: " + damage);
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if(healthPoints.value <= 0) {
                Experience exp = instigator.GetComponent<Experience>();
                if(exp!=null){
                    exp.GainExperince(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
                Die();
            }
            currentPercentage = GetPercentage();
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

        private void Die()
        {
            if(isDead) return;
                isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
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

