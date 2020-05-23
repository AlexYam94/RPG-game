using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {

        [SerializeField]
        float maxSpeed = 6f;

        private Ray lastRay;

        NavMeshAgent navMeshAgent;
        Health health;

        // Start is called before the first frame update
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            //navMeshAgent.destination = target.position;
            //if (Input.GetMouseButtonDown(0)||Input.GetMouseButton(0))
            //{
            //    MoveToCursor();
            //}
            if(health.IsDead())
                navMeshAgent.enabled = false;
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction){
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped=false;
        }

        public void Cancel(){
            navMeshAgent.isStopped=true;
        }

        public object CaptureState()
        {
            // return new SerializableVector3(transform.position);
            Dictionary<String,object> state = new Dictionary<String,object>();
            // state.Add("position",new SerializableVector3(transform.position));
            // state.Add("rotation",new SerializableVector3(transform.eulerAngles));
            return state;
        }

        public void RestoreState(object state)
        {
            // SerializableVector3 position = (SerializableVector3)state;
            // gameObject.GetComponent<NavMeshAgent>().enabled=false;
            // transform.position = position.ToVector();
            // gameObject.GetComponent<NavMeshAgent>().enabled=true;
            Dictionary<String, object> tempState = (Dictionary<String,object>)state;
            gameObject.GetComponent<NavMeshAgent>().enabled=false;
            // transform.position = ((SerializableVector3)tempState["position"]).ToVector();
            // transform.eulerAngles = ((SerializableVector3)tempState["rotation"]).ToVector();
            gameObject.GetComponent<NavMeshAgent>().enabled=true;
        }
    }
}