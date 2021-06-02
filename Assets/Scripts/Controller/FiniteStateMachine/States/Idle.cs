using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control{
    public class Idle : IState{
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public Idle(NavMeshAgent navMeshAgent, Animator animator){
            _navMeshAgent = navMeshAgent;
            _animator = animator;
        }

        public void OnEnter(){
            // _navMeshAgent.enabled = false;
            _animator.SetFloat("forwardSpeed",0);
        }

        public void Tick(){
            
        }

        public void OnExit(){
            // _navMeshAgent.enabled = true;
        }
    }
}