using RPG.Attributes;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat
{
    public interface IAttackControllerBehaviour
    {
        Animator anim { get; set; }

        public void Init(float timeBetweenAttacks, float attackSpeed, int maxNumberOfAttack, Animator anim, IAttribute stamina, MonoBehaviour coroutineRunner);

        public void CheckAttackInput();

        public void Attack();
    }
}