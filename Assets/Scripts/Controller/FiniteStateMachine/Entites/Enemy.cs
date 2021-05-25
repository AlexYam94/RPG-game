using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control{
    public class Enemy : MonoBehaviour, ICharacter, IEntity{

        [SerializeField] float moveSpeed;
        [SerializeField] float chaseSpeed;
        
        private float attackDistance;
        private StateMachine _stateMachine;
        private ICharacter target;

        private void Awake() {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var animator = GetComponent<Animator>();
            var playerDetector = GetComponent<TargetDetector<PlayerController>>();
            
            _stateMachine = new StateMachine();

            var moveToPlayer = new MoveToTarget(navMeshAgent, playerDetector, animator,moveSpeed, chaseSpeed, attackDistance, transform);
            var idle = new Idle(navMeshAgent, animator);

            Func<bool> PlayerInRange() => () => playerDetector.TargetInRange;
            Func<bool> PlayerNotInRange() => () => !playerDetector.TargetInRange;

            _stateMachine.AddAnyTransition(moveToPlayer,PlayerInRange());

            FromToWhere(moveToPlayer, idle, PlayerNotInRange());
        }
        
        public void FromToWhere(IState from, IState to, Func<bool> where)
        {
            _stateMachine.AddTransition(from, to, where);
        }

        public Health GetHealthComponent()
        {
            throw new NotImplementedException();
        }

        public Fighter GetFighterComponent()
        {
            throw new NotImplementedException();
        }

        public void HandleMovement()
        {
            throw new NotImplementedException();
        }

        public void HandleCombat()
        {
            throw new NotImplementedException();
        }

        public void HandleRotation()
        {
            throw new NotImplementedException();
        }

        public void InteractWithComponent()
        {
            throw new NotImplementedException();
        }

        public GameObject GetGameObject()
        {
            throw new NotImplementedException();
        }

        public string GetTag()
        {
            throw new NotImplementedException();
        }

    }
}