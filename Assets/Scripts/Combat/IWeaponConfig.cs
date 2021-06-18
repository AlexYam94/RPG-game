using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public interface IWeaponConfig
    {
        bool HasProjectile();
        Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator, ICharacter instigator);
        bool IsDual();
        Weapon GetSubWeapon();
    }
}