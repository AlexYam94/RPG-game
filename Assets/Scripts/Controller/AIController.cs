using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
using System;
using UnityEngine.AI;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        float chaseDistance = 5f;
        [SerializeField]
        float suspicionTime = 3f;
        [SerializeField]
        PatrolPath patrolPath;
        [SerializeField]
        float waypointTolerance = 1f;
        [SerializeField]
        float waypointDewllTime = 3f;

        [SerializeField]
        [Range(0,1)]
        float patrolSpeedFraction = 0.6f;

        Mover mover;
        Fighter fighter;
        Health health;
        GameObject player;
        LazyValue<Vector3> guardPostion;
        int currentWaypointIndex = 0;

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArriveAtWaypoint = Mathf.Infinity;

        readonly float chaseSpeed = 5f;
        readonly float patrolSpeed = 3f;

        private void Awake() {
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
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

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                GetComponent<NavMeshAgent>().speed = chaseSpeed;
                AttackBehaviour();
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

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArriveAtWaypoint += Time.deltaTime;
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

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer <= chaseDistance;
        }
    }
}

