using RPG.Attributes;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat
{
    public interface IAttackControllerBehaviour
    {
        Animator anim { get; set; }

        void Init(float timeBetweenAttacks, float attackSpeed, int maxNumberOfAttack, IAttribute stamina, MonoBehaviour coroutineRunner);

        void CheckAttackInput();

        void Attack();
    }
}