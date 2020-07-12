using System;
using System.Collections;
using UnityEngine;
using RPG.Resources;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField]
        AnimatorOverrideController weaponOverride = null;

        [SerializeField]
        GameObject equippedPrefab = null;

        [SerializeField]
        float range = 2f;

        [SerializeField]
        float weaponDamage = 1f;

        [SerializeField]
        bool isRightHanded = true;

        [SerializeField]
        Projectile projectile = null;

        const string weaponName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator){
            DestroyOldWeapon(rightHand,leftHand);

            Transform handTransform;
            if(equippedPrefab!=null)
            {
                handTransform = GetHandTransform(rightHand, leftHand);
                GameObject weapon = GameObject.Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride!=null){
                animator.runtimeAnimatorController = weaponOverride;
            }else if(overrideController!=null){
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null){
                oldWeapon = leftHand.Find(weaponName);
                if(oldWeapon == null)
                    return;
                oldWeapon.name = "Destroying";
                Destroy(oldWeapon.gameObject);
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator){

            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand,leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, weaponDamage);

        }
        
        public float getRange(){
            return range;
        }

        public void setRange(float range){
            this.range = range;
        }

        public float getDamage(){
            return weaponDamage;
        }

        public void setDamage(float damage){
            this.weaponDamage = damage;
        }

    }
}