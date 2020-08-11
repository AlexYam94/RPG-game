using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Resources;
using RPG.Stats;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField]
        [Range(0, 100)]
        int hitRate;

        [SerializeField]
        float timeBetweenAttacks = 0.1f;

        [SerializeField]
        Transform rightHandTransform = null;


        [SerializeField]
        Transform leftHandTransform = null;

        [SerializeField]
        Weapon defaultWeapon = null;
        
        [SerializeField]
        string defaultWeaponName = "Unarmed";

        float timeSinceAttack = Mathf.Infinity;
        Health target;
        LazyValue<Weapon> currentWeapon = null;

        private void Awake() {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        // Start is called before the first frame update
        void Start()
        {
            currentWeapon.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead())
            {
                GetComponent<Animator>().ResetTrigger("attack");
                target = null;
                return;
            }

            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else if (target)
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if (timeBetweenAttacks < timeSinceAttack)
            {
                //This will trigger the Hit() event
                transform.LookAt(target.transform);
                TriggetAttack();
                timeSinceAttack = 0f;

            }
        }

        private void TriggetAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animation effect
        void Hit()
        {
            if (target == null) return;
            bool hit = new System.Random().Next(0, 100) < hitRate;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value.hasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject,damage);
            }
            else
            {
                if (hit)
                {
                    target.TakeDamage(gameObject, damage);
                }
                // else
                // print("MISS");
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.value.GetRange(); ;
        }

        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (combatTarget == this.gameObject) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null & !targetToTest.IsDead();
        }

        public void Cancel()
        {
            StopAttack();
            // GetComponent<Animator>().enabled=false;
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget(){
            return target;
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage){
                yield return currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat){
            if(stat == Stat.Damage){
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }
    }
}