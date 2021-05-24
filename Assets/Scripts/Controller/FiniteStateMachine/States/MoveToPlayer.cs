using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class MoveToPlayer : IState
    {
        private readonly ICharacter _character;
        private NavMeshAgent _navMeshAgent;
        private readonly TargetDetector _targetDetector;
        private Animator _animator;
        private float _moveSpeed;
        private float _chaseSpeed;
        private float _chaseDistance;
        private float _attackDistance;
        private Transform _transform;

        public MoveToPlayer(ICharacter character, NavMeshAgent navMeshAgent, TargetDetector targetDetector, Animator animator, float moveSpeed, float chaseSpeed, float chaseDistance, float attackDistance, Transform transform){
            _character = character;
            _targetDetector = targetDetector;
            _animator = animator;
            _moveSpeed = moveSpeed;
            _chaseSpeed = chaseSpeed;
            _chaseDistance = chaseDistance;
            _attackDistance = attackDistance;
            _transform = transform;
        }

        public void OnEnter()
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.speed = _chaseSpeed;
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = _transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }

        public void Tick()
        {
            if(_navMeshAgent.remainingDistance <= _attackDistance) return;
            Vector3 targetPosition = _targetDetector.GetTargetPosition();
            _navMeshAgent.SetDestination(targetPosition);
        }


        public void OnExit()
        {
            _navMeshAgent.enabled = false;
            _navMeshAgent.speed = _moveSpeed;
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = _transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }
    }
}