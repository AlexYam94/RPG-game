using System.Collections;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control{
    public class Attack : MonoBehaviour, IState {
        //reference of damagerDealer for this entity
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Stamina _stamina;
        private int _maxNumberOfAttack;
        private int _currentAttackNumber = 1;
        private float _timeBetweenAttack;
        private float _timeSinceAttack = 0f;

        public Attack(NavMeshAgent navMeshAgent, Animator animator, Stamina stamina, int maxNumberOfAttack, float timeBetweenAttack){
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _stamina = stamina;
            _maxNumberOfAttack = maxNumberOfAttack;
            _timeBetweenAttack = timeBetweenAttack;
        }

        public void OnEnter(){
            _navMeshAgent.enabled = false;
            // StopAttack();
        }

        public void Tick(){
            if((_timeBetweenAttack < _timeSinceAttack) && _currentAttackNumber < _maxNumberOfAttack){
            //trigger attack animation
            //wait for animation finishe(need coroutine for this)
            //increase numberOfAttack
            StartCoroutine(AttackBehaviour());
            }
        }

        public void OnExit(){
            _navMeshAgent.enabled = true;
            // StopAttack();
        }

        private IEnumerator AttackBehaviour(){
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attack"+_currentAttackNumber);
            yield return null;
            // isAttack = true;
            // yield return new WaitForSeconds(attackTime);
            // isAttack = false;
        }

        private void StopAttack(){
            for(int i = 1;i<=_maxNumberOfAttack;i++){
                _animator.ResetTrigger("attack"+i);
            }
            _animator.SetTrigger("stopAttack");
        }
    }
}