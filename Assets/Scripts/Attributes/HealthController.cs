using GameDevTV.Saving;
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
        [SerializeField] MonoBehaviour[] componentsToDisbaleOnDie;

        bool isDead = false;
        float currentPercentage = 1f;

        HealthControllerBehaviour behaviour;

        private void Awake() {
            behaviour = new HealthControllerBehaviour(healthRegenerationOnLevelUp, GetComponent<Animator>(), GetComponent<ActionScheduler>(), GetComponents<Collider>(), componentsToDisbaleOnDie, GetComponent<BaseStats>());
        }

        private void Update()
        {
            behaviour.CheckDeath();
        }

        public bool IsDead(){
            return behaviour.IsDead();
        }

        public void ApplyDamage(float damage)
        {
            behaviour.ApplyDamage(damage);
        }

        public float GetPercentage(){
            return behaviour.GetPercentage();
        }

        public float GetFraction(){
            return behaviour.GetFraction();
        }

        public bool isEnable()
        {
            return this.enabled;
        }

        public object CaptureState()
        {
            return behaviour;
        }

        public void RestoreState(object state)
        {
            behaviour = (HealthControllerBehaviour)state;
            if(behaviour.healthPoints.value <= 0) behaviour.Die();
        }
    }
}