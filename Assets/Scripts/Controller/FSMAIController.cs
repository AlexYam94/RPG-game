using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using System;
using UnityEngine.AI;
using GameDevTV.Utils;
using RPG.Stats;
using RPG.Tool;
using RPG.Control;

namespace Test.Control
{
    public class FSMAIController : MonoBehaviour, IFiniteStateMachine
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float agroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDewllTime = 3f;
        [SerializeField] float shoutDistance = 4;
        [SerializeField] AggressiveLevel aggressiveLevelDict;
        [SerializeField] AggressiveLevel.AggressiveLevelEnum aggressiveLevel;
        [SerializeField] float backwardDistance = .1f;
        [SerializeField] float backwardSpeed = .1f;
        [SerializeField] float chaseSpeed = 5f;
        [SerializeField] float patrolSpeed = 3f;
        [SerializeField][Range(0,1)]float patrolSpeedFraction = 0.6f;
        Stamina stamina;
        NavMeshAgent navMeshAgent;
        Mover mover;
        Fighter fighter;
        Health health;
        GameObject player;
        LazyValue<Vector3> guardPostion;
        int currentWaypointIndex = 0;
        bool isAttacking = false;
        Animator anim;

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArriveAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        float attackTime;

        [SerializeField]
        private float stateRoutineIteration = 0.15f;

        private AIStates currentState = AIStates.IDLE;
        private AIStates oldState;


        /*************************************************************************
        *********
        *********        public functions                                          
        *********
        *************************************************************************/

        public IEnumerator StateMachine()
        {
            while (!health.IsDead())
            {
                yield return new WaitForSeconds(stateRoutineIteration);
                // if(currentState == oldState) continue;
                oldState = currentState;
                switch (currentState)
                {
                    case AIStates.IDLE:
                        Idle();
                        break;
                    case AIStates.ALERT:
                        Alert();
                        break;
                    case AIStates.CHASE:
                        Chase();
                        break;
                    case AIStates.ATTACK:
                        Attack();
                        break;
                    case AIStates.PATROL:
                        Patrol();
                        break;
                    case AIStates.WANDER:
                        Idle();
                        break;
                }
                yield return null;
            }
        }


        /*************************************************************************
        *********
        *********        private functions                                          
        *********
        *************************************************************************/

        private void Awake(){
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            stamina = GetComponent<Stamina>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            guardPostion = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start(){
            guardPostion.ForceInit();
            anim = GetComponent<Animator>();
            StartCoroutine(StateMachine());
        }

        private void Update(){
            UpdateTimers();
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }
        private void Idle()
        {
            //check if player is in chase Range
            //if so change state to Chase
            if(IsAggrevated()){
                currentState = AIStates.CHASE;
                return;
            }   
            // mover.StartMoveAction(guardPostion.value,patrolSpeedFraction);
            GetComponent<ActionScheduler>().CancelCurrentAction();

        }

        private void Alert()
        {
            //play alert animation
            //enter wander state to search player?
        }

        private void Chase()
        {      
            //look at player
            //if in attack range, go to attack state
            //if stamina is low, go to wander state to recover state
            //approach player if stamina is high enough and not in attack range
            transform.LookAt(player.transform);
            navMeshAgent.speed = chaseSpeed;
            if(fighter.CanAttack(player)){
                currentState = AIStates.ATTACK;
                return;
            }
            if(stamina.GetPercentage()<=aggressiveLevelDict.lookUp(aggressiveLevel)){
                currentState = AIStates.WANDER;
                return;
            }
            // mover.MovetTo(player.transform.position,1f);
            mover.StartMoveAction(player.transform.position,patrolSpeedFraction);

        }

        private void Attack()
        {
            // if(isAttacking) return;
            if(fighter.CanAttack(player)){
                AttackBehaviour();
            }else{
                currentState = AIStates.CHASE;
            }

        }

        private void Patrol()
        {

        }

        

        private void SetNavMeshAgentZVelocity(float z)
        {
            Vector3 velocity = navMeshAgent.velocity;
            velocity.z = z;
            navMeshAgent.velocity = velocity;
        }

        private void Aggrevate(){
            timeSinceAggrevated = 0f;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArriveAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
            
        }

        private void PatroBehaviour()
        {
            Vector3 nextPosition = guardPostion.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArriveAtWaypoint = 0f;
                    CycleWaypoint();
                }
                else
                {
                    nextPosition = GetCurrentWaypoint();
                }
            }
            if (timeSinceArriveAtWaypoint > waypointDewllTime)
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint <= waypointTolerance;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
            StartCoroutine("WaitAttackFinish");
        }

        private IEnumerator WaitAttackFinish(){
            attackTime = Util.GetCurrentAnimationTime(attackTime,"attack",anim);
            isAttacking=true;
            yield return new WaitForSeconds(attackTime);
            isAttacking=false;
            // fighter.DisableTrigger();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up,0);
            foreach(RaycastHit hit in hits){
                AIController ai = hit.collider.GetComponent<AIController>();
                if(ai!=null)
                    ai.Aggrevate();
            }
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private bool IsSuspicious()
        {
            return timeSinceLastSawPlayer <= suspicionTime;
        }

        //Called by unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return (distanceToPlayer <= chaseDistance) || (timeSinceAggrevated < agroCooldownTime);
        }

    }


    public enum AIStates
    {
        IDLE,
        ALERT,
        CHASE,
        ATTACK,
        PATROL,
        WANDER
    }
}