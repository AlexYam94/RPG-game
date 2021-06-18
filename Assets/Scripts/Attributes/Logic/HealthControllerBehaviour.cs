using GameDevTV.Utils;
using RPG.Core;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class HealthControllerBehaviour
    {
        public LazyValue<float> healthPoints { get; private set; }

        float _healthRegenerationOnLevelUp = 0f;
        Animator _anim;
        ActionScheduler _actionScheduler;
        Collider[] _colliders;
        MonoBehaviour[] _components;
        IBaseStats _stat;
        UnityEvent onDie;

        bool _isDead = false;
        float _currentPercentage = 1f;


        public HealthControllerBehaviour(float healthRegenerationOnLevelUp, Animator anim, ActionScheduler actionScheduler, Collider[] colliders, MonoBehaviour[] components, IBaseStats stat)
        {
            _healthRegenerationOnLevelUp = healthRegenerationOnLevelUp;
            _anim = anim;
            _actionScheduler = actionScheduler;
            _colliders = colliders;
            _components = components;
            _stat = stat;
        }
        private float GetInitialHealth()
        {
            return _stat.GetStat(Stat.Health);
        }

        public void init()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            _stat.onLevelUp += UpdateHealth;
        }

        public void UpdateHealth()
        {
            float nextLevelMaxHealth = _stat.GetStat(Stat.Health);
            float currentHealthPercentage = Mathf.Clamp(_currentPercentage, 0, 100);
            float nextLevelHealth = nextLevelMaxHealth * (currentHealthPercentage + _healthRegenerationOnLevelUp / 100);
            healthPoints.value = Mathf.Clamp(nextLevelHealth, nextLevelHealth, nextLevelMaxHealth);
        }

        public void Die()
        {
            if (_isDead) return;
            _isDead = true;
            if(_anim!=null) _anim.SetTrigger("die");
            if (_actionScheduler != null) _actionScheduler.CancelCurrentAction();
            //foreach (var item in _colliders)
            //{
            //    item.enabled = false;
            //}
            //foreach (var item in _components)
            //{
            //    item.enabled = false;
            //}
            if(_colliders!=null)
                new List<Collider>(_colliders).ForEach(item => item.enabled = false);
            if(_components!=null)    
                new List<MonoBehaviour>(_components).ForEach(item => item.enabled = false);
        }

        public void CheckDeath()
        {
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void ApplyDamage(float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
        }

        public float GetPercentage()
        {
            return healthPoints.value / _stat.GetStat(Stat.Health) * 100;
        }

        public float GetFraction()
        {
            return healthPoints.value / _stat.GetStat(Stat.Health);
        }

    }
}
