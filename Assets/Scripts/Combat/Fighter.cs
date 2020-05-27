using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        [Range(0, 100)]
        int hitRate;

        [SerializeField]
        float timeBetweenAttacks = 0.1f;

        [SerializeField]
        Transform handTransform = null;

        [SerializeField]
        Weapon defaultWeapon = null;


        float timeSinceAttack = Mathf.Infinity;
        Health target;
        Weapon currentWeapon = null;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceAttack += Time.deltaTime;
            if (target == null) return;
            if(target.IsDead()){
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
                // GetComponent<Mover>().Cancel();
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
            if(target==null)return;
            bool hit = new System.Random().Next(0, 100) < hitRate;
            if (hit)
                target.TakeDamage(currentWeapon.getDamage());
            else
                print("MISS");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.getRange();;
        }

        public void Attack(GameObject combatTarget)
        {
            print("Attack");
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget==null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest!=null&!targetToTest.IsDead();
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

        public void EquipWeapon(Weapon weapon){
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(handTransform,animator);
        }
    }
}