using UnityEngine;

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
        float damage = 1f;

        public void Spawn(Transform handTransform, Animator animator){
            if(equippedPrefab!=null){
                GameObject.Instantiate(equippedPrefab,handTransform);
            }
            if(weaponOverride!=null)
                animator.runtimeAnimatorController = weaponOverride;
            
        }
        
        public float getRange(){
            return range;
        }

        public void setRange(float range){
            this.range = range;
        }

        public float getDamage(){
            return damage;
        }

        public void setDamage(float damage){
            this.damage = damage;
        }

    }
}