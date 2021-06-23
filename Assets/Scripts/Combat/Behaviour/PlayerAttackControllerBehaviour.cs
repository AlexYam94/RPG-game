using RPG.Attributes;
using RPG.Tool;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat
{
    public class PlayerAttackControllerBehaviour : IAttackControllerBehaviour
    {
        float _timeBetweenAttacks = 1f;
        float _attackSpeed = 1f;
        int _maxNumberOfAttack = 3;
        float _timeSinceAttack = Mathf.Infinity;
        bool _isAttacking = false;
        float _attackTime = 0f;
        Animator _anim;
        int _numberOfAttack = 0;
        int _tempMaxNumberOfAttack = 1;
        Vector3 _shootDirection;
        IAttribute _stamina;
        MonoBehaviour _monoBehaviour;
        bool _startAttack = false;

        Animator IAttackControllerBehaviour.anim { get => _anim; set => _anim = value; }

        public void Init(float timeBetweenAttacks, float attackSpeed, int maxNumberOfAttack, IAttribute stamina, MonoBehaviour monoBehaviour)
        {
            _timeBetweenAttacks = timeBetweenAttacks;
            _attackSpeed = attackSpeed;
            _maxNumberOfAttack = maxNumberOfAttack;
            _stamina = stamina;
            _monoBehaviour = monoBehaviour;
            _anim = monoBehaviour.GetComponent<Animator>();
        }

        public void CheckAttackInput()
        {
            if (Input.GetMouseButtonDown(0) && !_isAttacking)
            {
                _startAttack = true;
            }
        }

        public void Attack()
        {
            if (_startAttack)
            {
                HandleAttack();
            }
        }


        private void HandleAttack()
        {
            if (_timeBetweenAttacks < _timeSinceAttack && _numberOfAttack < _maxNumberOfAttack)
            {
                if (_stamina.GetPercentage() > 0)
                {
                    _stamina.Reduce(20f);
                    //TODO:Cancel movement
                    AttackBehaviour();
                }
            }
        }

        private void AttackBehaviour()
        {
                _numberOfAttack = Mathf.Clamp(_numberOfAttack + 1, 1, _maxNumberOfAttack);
            _monoBehaviour.StartCoroutine(TriggerAttackAnimation());
        }

        private IEnumerator TriggerAttackAnimation()
        {
            int tempAttckNum = _numberOfAttack;
            _anim.ResetTrigger("stopAttack");
            _anim.SetTrigger("attack" + tempAttckNum);
            _timeSinceAttack = 0f;
            _attackTime = Util.GetCurrentAnimationTime(_attackTime, "attack", _anim) / _attackSpeed;
            _isAttacking = true;
            yield return new WaitForSeconds(_attackTime);
            _isAttacking = false;
            _anim.ResetTrigger("attack" + tempAttckNum);
            _anim.SetTrigger("stopAttack");
        }
    }
}