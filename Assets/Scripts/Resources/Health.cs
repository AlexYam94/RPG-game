using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using RPG.Combat;
using System;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField][Range(0, 100)] float healthRegenerationOnLevelUp = 1f;

        float health = -1f;
        bool isDead;
        float currentPercentage = -1f;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<BaseStats>().onLevelUp += updateHealth;
            if(health < 0){
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }



        private void updateHealth()
        {
            float tempHealth = GetComponent<BaseStats>().GetStat(Stat.Health) * (Mathf.Clamp(currentPercentage,0,100) + healthRegenerationOnLevelUp)/100;
            health = tempHealth;
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isDead) return;
            print(gameObject.name + " took damage: " + damage);
            health = Mathf.Max(health - damage, 0);
            if(health <= 0) {
                Experience exp = instigator.GetComponent<Experience>();
                if(exp!=null){
                    exp.GainExperince(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
                Die();
            }
            currentPercentage = GetPercentage();
        }

        public float GetHealthPoints(){
            return health;
        }

        public float GetMaxHealthPoints(){
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage(){
            return health/GetComponent<BaseStats>().GetStat(Stat.Health) * 100;
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
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            if(health <= 0) Die();
        }
    }
}

