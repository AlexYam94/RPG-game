using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using System;
using RPG.Tool;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] [Range(0, 100)] int hitRate;

        [SerializeField] float timeBetweenAttacks = 0.1f;

        [SerializeField] Transform rightHandTransform = null;


        [SerializeField] Transform leftHandTransform = null;

        [SerializeField] WeaponConfig defaultWeapon = null;
        
        [SerializeField] string defaultWeaponName = "Unarmed";
        [Range(1,3)][SerializeField] float attackSpeed = 1f;

        float timeSinceAttack = Mathf.Infinity;
        Health target;
        WeaponConfig currentWeaponConfig = null;
        LazyValue<Weapon> currentWeapon;
        Stamina stamina = null;
        private bool isAttacking = false;
        float attackTime = 0f;
        Animator anim = null;

        private void Awake() {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            currentWeaponConfig = defaultWeapon;
            stamina = GetComponent<Stamina>();
            anim = GetComponent<Animator>();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        // Start is called before the first frame update
        void Start()
        {
            currentWeapon.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Animator>().SetFloat("attackSpeed", attackSpeed);
            timeSinceAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead())
            {
                GetComponent<Animator>().ResetTrigger("attack");
                target = null;
                return;
            }

            if (!GetIsInRange(target.transform)&&!isAttacking)
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else if (target)
            {
                GetComponent<Mover>().Cancel();
                if(stamina.HasStaminaLeft()){
                    AttackBehaviour();
                }
            }
        }

        private void AttackBehaviour()
        {
            if (timeBetweenAttacks < timeSinceAttack)
            {
                //This will trigger the Hit() event
                transform.LookAt(target.transform);
                StartCoroutine("TriggerAttack");
                stamina.ConsumeStaminaOnce(20f,Stamina.StaminaType.attack);
                timeSinceAttack = 0f;

            }
        }

        private IEnumerator TriggerAttack(){
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
            isAttacking=true;
            yield return new WaitForSeconds(attackTime);
            isAttacking=false;
        }

        // private void TriggerAttack()
        // {
        //     GetComponent<Animator>().ResetTrigger("stopAttack");
        //     GetComponent<Animator>().SetTrigger("attack");
        // }

        private void OnTriggerEnter(Collider other)
        {
            if (this.gameObject.tag.Equals("Player"))
            {
                HandlePlayerHit(other);
            }
            else
            {
                if (target == null) return;
                if (!currentWeapon.value.GetCanTrigger()) return;
                if (target != other.gameObject.GetComponent<Health>()) return;
                bool hit = new System.Random().Next(0, 100) < hitRate;
                float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

                if (currentWeapon.value != null)
                {
                    currentWeapon.value.OnHit();
                }

                if (currentWeaponConfig.hasProjectile())
                {
                    currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
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
        }

        private void HandlePlayerHit(Collider other) {
            bool canTrigger = currentWeapon.value.GetCanTrigger();
            // print("OnTriggerEnter canTrigger: " + canTrigger);
            if(!canTrigger){
                return;
            }
            // print("trigger");
            // print(other.gameObject.name);
            Health target = other.gameObject.GetComponent<Health>();
            if(target == null) return;
            float damage = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().GetStat(Stat.Damage);
            target.TakeDamage(GameObject.FindGameObjectWithTag("Player"), damage);
        }

        // //Animation effect
        // void Hit()
        // {
        //     if (target == null) return;
        //     bool hit = new System.Random().Next(0, 100) < hitRate;
        //     float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

        //     if(currentWeapon.value!=null){
        //         currentWeapon.value.OnHit();
        //     }

        //     if (currentWeaponConfig.hasProjectile())
        //     {
        //         currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject,damage);
        //     }
        //     else
        //     {
        //         if (hit)
        //         {
        //             target.TakeDamage(gameObject, damage);
        //         }
        //         // else
        //         // print("MISS");
        //     }
        // }

        void Shoot()
        {
            // Hit();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetRange(); ;
        }

        public void Attack(GameObject combatTarget)
        {
            attackTime = Util.GetCurrentAnimationTime(attackTime,"attack",anim);
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) {return false;}
            if (combatTarget == this.gameObject) {return false;}
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position)&&!GetIsInRange(combatTarget.transform)){ 
                return false;
            }
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

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
            if(GetComponent<IKController>()!= null){
                GetComponent<IKController>().SetGrabObj(currentWeapon.value.GetGrabObj());
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget(){
            return target;
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage){
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat){
            if(stat == Stat.Damage){
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        public void EnableTrigger(){
            currentWeaponConfig.EnableTrigger();
        }

        public void DisableTrigger(){
            currentWeaponConfig.DisableTrigger();
        }

        public void EnableWeaponTrigger(){
            currentWeapon.value.EnableTrigger();
            // agent.SetDestination(transform.position);
            // agent.enabled = false;
        }

        public void DisableWeaponTrigger(){
            currentWeapon.value.DisableTrigger();
            // agent.enabled = true;
        }

        public float GetBlockingAngle(){
            return currentWeaponConfig.GetBlockingAngle();
        }
    }
}