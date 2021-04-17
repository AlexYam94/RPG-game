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

namespace RPG.Control
{
    public class AIController : MonoBehaviour
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

        [SerializeField]
        [Range(0,1)]
        float patrolSpeedFraction = 0.6f;
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

        private void Awake() {
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            stamina = GetComponent<Stamina>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            guardPostion = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {
            guardPostion.ForceInit();
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            // if(!InAttackRangeOfPlayer()){
            //     print(gameObject.name + " Chase");
            //     mover.MoveTo(player.transform.position);
            // }
            // else if(fighter.CanAttack(player))
            // {
            //     fighter.Attack(player);
            // }
            if (health.IsDead()) return;
            if(isAttacking) return;

            if (IsAggrevated())
            {
                Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
                transform.LookAt(player.transform);
                navMeshAgent.speed = chaseSpeed;
                if(stamina.GetPercentage()<=aggressiveLevelDict.lookUp(aggressiveLevel)&&!isAttacking)
                {
                    //set destination to self?
                    // SetNavMeshAgentZVelocity(-1);
                    mover.MoveTo(transform.position - (transform.forward * backwardDistance),backwardSpeed);
                }
                else if(fighter.CanAttack(player)){
                    // if(stamina.HasStaminaLeft()){
                        // mover.Cancel();
                        AttackBehaviour();
                    // }
                }else{
                    mover.MoveTo(player.transform.position,1f);
                }
            }
            else if (IsSuspicious())
            {
                SuspicionBehaviour();
            }
            else
            {
                GetComponent<NavMeshAgent>().speed = patrolSpeed;
                PatroBehaviour();
            }

            UpdateTimers();

        }

        private void SetNavMeshAgentZVelocity(float z)
        {
            Vector3 velocity = navMeshAgent.velocity;
            velocity.z = z;
            navMeshAgent.velocity = velocity;
        }

        public void Aggrevate(){
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
            StartCoroutine("StartAttack");
        }

        public IEnumerator StartAttack(){
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

        // public void EnableWeaponTrigger(){
        //     print("Enable weapon trigger");
        //     fighter.EnableTrigger();
        //     // agent.SetDestination(transform.position);
        //     // agent.enabled = false;
        // }

        // public void DisableWeaponTrigger(){
        //     print("Disable weapon trigger");
        //     fighter.DisableTrigger();
        //     // agent.enabled = true;
        // }

    }
}

