using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using RPG.Combat;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {

        [SerializeField]
        float health = 100f;

        bool isDead;

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isDead) return;
            health = Mathf.Max(health - damage, 0);
            if(gameObject.tag == "Player"){
                GameObject.Find("Health Bar").GetComponent<HealthBarDisplay>().UpdateHealthBar();
            }else if(gameObject.tag == "Enemy"){
                GameObject.Find("Enemy Health Bar").GetComponent<EnemyHealthBarDisplay>().UpdateHealthBar();
            }
            if(health <= 0) {
                Experience exp = instigator.GetComponent<Experience>();
                print(exp);
                if(exp!=null){
                    exp.GainExperince(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
                Die();
            }
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

