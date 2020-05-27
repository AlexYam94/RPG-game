using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {

        [SerializeField]
        float health = 100f;

        bool isDead;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;
            health = Mathf.Max(health - damage, 0);
            if(health <= 0) Die();
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

