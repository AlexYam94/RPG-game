using RPG.Attributes;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyAttackControllerBehaviour : IAttackControllerBehaviour
    {
        public Animator anim { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void CheckAttackInput()
        {
            throw new System.NotImplementedException();
        }

        public void Init(float timeBetweenAttacks, float attackSpeed, int maxNumberOfAttack, IAttribute stamina, MonoBehaviour coroutineRunner)
        {
            throw new System.NotImplementedException();
        }
    }
}