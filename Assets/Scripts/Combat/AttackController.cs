using RPG.Attributes;
using RPG.Factory;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class AttackController : IAttackController
    {
        [SerializeField] AttackControllerType controllerType;
        [SerializeField] float timeBetweenAttacks = 0.1f;
        [SerializeField] float attackSpeed = 1f;
        [SerializeField] int maxNumberOfAttack = 3;

        IAttackControllerBehaviour _behaviour;
        IAttribute _stamina;
        Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _behaviour = AttackControllerFactory.GetInstance().create(controllerType);
        }

        private void Start()
        {
            _behaviour.Init(timeBetweenAttacks,attackSpeed,maxNumberOfAttack, _anim, _stamina, this);
        }

        private void Update()
        {
            _behaviour.CheckAttackInput();
        }

        private void FixedUpdate()
        {
            _behaviour.Attack();
        }
    }
}