using System;
using System.Collections;
using UnityEngine;
using RPG.Attributes;
using GameDevTV.Inventories;
using RPG.Core;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem
    {
        [SerializeField]
        AnimatorOverrideController weaponOverride = null;

        [SerializeField]
        Weapon equippedPrefab = null;

        [SerializeField]
        float range = 2f;

        [SerializeField]
        float weaponDamage = 1f;

        [SerializeField]
        bool isRightHanded = true;

        [SerializeField]
        bool isDual = false;

        [SerializeField]
        Projectile projectile = null;

        [SerializeField]
        float blockingAngle = 120f;

        [SerializeField]
        float percentageBonus = 5f;

        [SerializeField]
        float staminaPerHit = 15f;

        [SerializeField]
        float staminaSpecialAttack = 25f;

        const string weaponName = "Weapon";

        private Weapon currentWeaponInstance = null;
        private Weapon currentSubWeaponInstance = null;

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator){
            DestroyOldWeapon(rightHand,leftHand);

            Transform handTransform;
            Weapon weapon = null;
            if(equippedPrefab!=null)
            {
                handTransform = GetHandTransform(rightHand, leftHand);
                weapon = GameObject.Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            Weapon weapon2 = null;
            if(isDual){
                if(isRightHanded){
                    weapon2 = GameObject.Instantiate(equippedPrefab,leftHand);
                }else if(!isRightHanded){
                    weapon2 = GameObject.Instantiate(equippedPrefab,rightHand);
                }
                weapon2.gameObject.name = weaponName+"2";
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride!=null){
                animator.runtimeAnimatorController = weaponOverride;
            }else if(overrideController!=null){
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            currentWeaponInstance = weapon;
            if(weapon2 != null)
                currentSubWeaponInstance = weapon2;
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            Transform oldWeapon2 = leftHand.Find(weaponName);
            if(oldWeapon != null){
                oldWeapon.name = "Destroying";
                Destroy(oldWeapon.gameObject);
            }
            if(oldWeapon2 != null){
                oldWeapon2.name = "Destroying";
                Destroy(oldWeapon2.gameObject);
            }
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }

            return handTransform;
        }

        public bool hasProjectile(){
            return this.projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, ICharacter instigator, float calculatedDamage, Vector3 shootDirection){

            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand,leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
            projectileInstance.SetShootDirection(shootDirection);

        }

        public float GetPercentageBonus(){
            return percentageBonus;
        }
        
        public float GetRange(){
            return range;
        }

        public void SetRange(float range){
            this.range = range;
        }

        public float GetDamage(){
            return weaponDamage;
        }

        public void SetDamage(float damage){
            this.weaponDamage = damage;
        }

        public void EnableTrigger(){
            currentWeaponInstance.EnableTrigger();
        }

        public void DisableTrigger(){
            currentWeaponInstance.DisableTrigger();
        }
        
        public void EnableSubTrigger(){
            currentSubWeaponInstance.EnableTrigger();
        }

        public void DisableSubTrigger(){
            currentSubWeaponInstance.DisableTrigger();
        }

        public float GetBlockingAngle(){
            return blockingAngle;
        }

        public bool IsDual(){
            return isDual;
        }

        public Weapon GetSubWeapon(){
            return currentSubWeaponInstance;
        }

        public float GetStaminaPerHit(){
            return staminaPerHit;
        }
        
        public float GetStaminaSpecialAttack(){
            return staminaSpecialAttack;
        }
    }
}