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
        [SerializeField] int maxNumberOfAttack;
        [SerializeField] float timeBetweenAttack;
        
        private float _attackDistance;
        private AIStateMachine _actionStateMachine;
        private AIStateMachine _healthStateMachine;

        private void Awake() {
            
            InitStateMachine();

        }

        private void Update(){
            _actionStateMachine.Tick();
            _healthStateMachine.Tick();
        }

        public void InitStateMachine()
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var animator = GetComponent<Animator>();
            var playerDetector = GetComponent<TargetDetector<PlayerController>>();
            InitActionStateMachine(navMeshAgent, animator, playerDetector);
            InitHealthStateMachine();
        }

        private void InitActionStateMachine(NavMeshAgent navMeshAgent, Animator animator, TargetDetector<PlayerController> playerDetector){
            _actionStateMachine = new AIStateMachine();

            Fighter fighter = new Fighter();
            Stamina stamina = new Stamina();

            var attack = new Attack(navMeshAgent,animator, stamina, maxNumberOfAttack, timeBetweenAttack);
            var moveToPlayer = new MoveToTarget(navMeshAgent, playerDetector, animator, moveSpeed, chaseSpeed, 
            1f,//replace with weapon attack distance
             transform);
            var idle = new Idle(navMeshAgent, animator);


            Func<bool> CanAttack() => () => fighter.CanAttack(playerDetector.GetTarget());
            Func<bool> PlayerInRange() => () => playerDetector.TargetInRange;
            Func<bool> ShouldIdle() => () => {return !(fighter.CanAttack(playerDetector.GetTarget()) && playerDetector.TargetInRange);};

            _actionStateMachine.AddAnyTransition(attack, CanAttack());
            _actionStateMachine.AddAnyTransition(moveToPlayer, PlayerInRange());
            
            _actionStateMachine.AddTransition(moveToPlayer, idle, ShouldIdle());
        }
        
        private void InitHealthStateMachine(){
            _healthStateMachine = new AIStateMachine();
        }

        public void FromToWhere(IStateMachine stateMachine, IState from, IState to, Func<bool> where)
        {
            stateMachine.AddTransition(from, to, where);
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