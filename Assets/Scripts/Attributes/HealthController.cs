using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class HealthController : SerializedMonoBehaviour, ISaveable, IAttribute, IHealth
    {
        [SerializeField] [Range(0, 100)] float healthRegenerationOnLevelUp = 10f;
        [SerializeField] UnityEvent onDie;
        [SerializeField] [Range(0, 100)] float healthRegen = 0f;

        LazyValue<float> healthPoints;
        bool isDead = false;
        float currentPercentage = 1f;
        float damageOfTimeCounter = 0;
        float damagePerSec = 0;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            GetComponent<BaseStats>().onLevelUp += UpdateHealth;
        }
        
        private float GetInitialHealth(){
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void UpdateHealth()
        {
            float nextLevelMaxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            float currentHealthPercentage = Mathf.Clamp(currentPercentage,0,100);
            float nextLevelHealth = nextLevelMaxHealth * (currentHealthPercentage + healthRegenerationOnLevelUp/100);
            healthPoints.value = Mathf.Clamp(nextLevelHealth,nextLevelHealth,nextLevelMaxHealth);
        }
        
        private void Die()
        {
            if(isDead) return;
                isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            Collider[] colliders = GetComponents<Collider>();
            GetComponent<Stamina>().enabled = false;
            foreach (var item in colliders)
            {   
                item.enabled = false;
            }
        }
        
        public bool IsDead(){
            return isDead;
        }

        public void ApplyDamage(float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
        }

        public float GetPercentage(){
            return healthPoints.value/GetComponent<BaseStats>().GetStat(Stat.Health) * 100;
        }

        public float GetFraction(){
            return healthPoints.value/GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool isEnable()
        {
            return this.enabled;
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