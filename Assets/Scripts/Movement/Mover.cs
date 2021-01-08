using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {

        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;

        private Ray lastRay;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        // Start is called before the first frame update
        void Start()
        {
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
                if(navMeshAgent!=null)
                    navMeshAgent.enabled = false;
            if(this.gameObject.name!="Player")
                UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction){
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination){
            if(navMeshAgent!=null) return false;
            NavMeshPath path = new NavMeshPath();
            if(!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path)) return false;
            if(path.status != NavMeshPathStatus.PathComplete) return false;
            if(GetPathLength(path) > maxNavPathLength) return false;
            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            if(navMeshAgent==null) return;
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped=false;
        }

        public void Cancel(){
            if(navMeshAgent!=null) return;
            navMeshAgent.isStopped=true;
            navMeshAgent.destination = transform.position;
            navMeshAgent.SetDestination(gameObject.transform.position);
            navMeshAgent.speed = 0;
            navMeshAgent.velocity = Vector3.zero;
        }

        public object CaptureState()
        {
            // return new SerializableVector3(transform.position);
            Dictionary<String,object> state = new Dictionary<String,object>();
            state.Add("position",new SerializableVector3(transform.position));
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
            navMeshAgent.enabled=false;
            transform.position = ((SerializableVector3)tempState["position"]).ToVector();
            // transform.eulerAngles = ((SerializableVector3)tempState["rotation"]).ToVector();
            // navMeshAgent.enabled=true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            Vector3[] corners = path.corners;
            if(corners.Length<2) return total;
            for(int i=0; i<corners.Length-1;i++){
                total += Vector3.Distance(corners[i],corners[i+1]);
            }
            return total;
        }
    }
}