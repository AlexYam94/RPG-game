using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using System;
using RPG.Tool;
using Sirenix.OdinInspector;
using GameDevTV.Saving;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {

        [SerializeField] float timeBetweenAttacks = 0.1f;

        [SerializeField] Transform rightHandTransform = null;


        [SerializeField] Transform leftHandTransform = null;

        [SerializeField] WeaponConfig defaultWeapon = null;
        
        [SerializeField] string defaultWeaponName = "Unarmed";
        [Range(0,3)][SerializeField] float attackSpeed = 1f;

        [SerializeField] float maxTimeGapBetweenAttacks =2f;

        [SerializeField] int maxNumberOfAttack = 3;

        float timeSinceAttack = Mathf.Infinity;
        Health target;
        WeaponConfig currentWeaponConfig = null;
        LazyValue<Weapon> currentWeapon;
        LazyValue<Weapon> currentSubWeapon;
        Stamina stamina = null;
        private bool isAttacking = false;
        float attackTime = 0f;
        Animator anim = null;
        Mover mover = null;
        int numberOfAttack = 0;
        int tempMaxNumberOfAttack = 1;
        Equipment equipment;
        BaseStats baseStat;
        private Vector3 shootDirection;
        Armour armour;
        ICharacter character;

        private void Awake() {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            currentWeaponConfig = defaultWeapon;
            stamina = GetComponent<Stamina>();
            anim = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            equipment = GetComponent<Equipment>();
            if(equipment) {
                equipment.equipmentUpdated += UpdateWeapon;
            }
            baseStat = GetComponent<BaseStats>();
            armour = GetComponent<Armour>();
            character = GetComponent<ICharacter>();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
            // return EquipWeapon(defaultWeapon);
        }

        // Start is called before the first frame update
        void Start()
        {
            currentWeapon.ForceInit();
            // equipment.AddItem(EquipLocation.Weapon,currentWeaponConfig);
        }

        // Update is called once per frame
        void Update()
        {
            anim.SetFloat("attackSpeed", attackSpeed);
            timeSinceAttack += Time.deltaTime;
            // print("timeSinceAttack: " + timeSinceAttack);
            if(numberOfAttack>maxNumberOfAttack||timeSinceAttack>maxTimeGapBetweenAttacks){
                tempMaxNumberOfAttack = 1;
                numberOfAttack = 0;
                StopAttack();
            }
            if (target == null) return;
            if (target.IsDead())
            {
                StopAttack();
                target = null;
                return;
            }
            
            // if (!GetIsInRange(target.transform)&&!isAttacking)
            // {
            //     mover.MoveTo(target.transform.position, 1f);
            // }
            // else if (target)
            if (GetIsInRange(target.transform)&&!isAttacking&&target)
            {
                mover.Cancel();
                if(stamina.HasStaminaLeft()){
                    AttackBehaviour(20f);
                }
            }
        }

        private void AttackBehaviour()
        {
            if (timeBetweenAttacks < timeSinceAttack&&numberOfAttack<maxNumberOfAttack)
            {
                // if(timeSinceAttack < maxTimeGapBetweenAttacks){
                    numberOfAttack = Mathf.Clamp(numberOfAttack+1,1,maxNumberOfAttack);
                // }else{
                    // numberOfAttack = 1;
                // }
                //This will trigger the Hit() event
                transform.LookAt(target.transform);
                StartCoroutine("TriggerAttack");
                stamina.ConsumeStaminaOnce(20f,Stamina.StaminaType.attack);
                timeSinceAttack = 0f;
            }
        }
     
        private void AttackBehaviour(float staminaToConsume)
        {
            if (timeBetweenAttacks < timeSinceAttack&&numberOfAttack<3)
            {
                // if(timeSinceAttack < maxTimeGapBetweenAttacks){
                    numberOfAttack = Mathf.Clamp(numberOfAttack+1,1,3);
                // }else{
                    // numberOfAttack = 1;
                // }
                //This will trigger the Hit() event
                StartCoroutine("TriggerAttack");
                stamina.ConsumeStaminaOnce(staminaToConsume,Stamina.StaminaType.attack);
                timeSinceAttack = 0f;
            }
        }

        private IEnumerator TriggerSpecialAttack(){
            // print(gameObject.name + ":  attack" + numberOfAttack);
            anim.ResetTrigger("stopAttack");
            anim.SetBool("isAttacking",true);
            anim.SetTrigger("speicalAttack");
            if(attackTime>0){
                isAttacking=true;
                yield return new WaitForSeconds(attackTime);
                isAttacking=false;
            }
            // anim.SetTrigger("stopAttack");
        }

        private IEnumerator TriggerAttack(){
            // print(gameObject.name + ": attack" + numberOfAttack);
            int tempAttckNum = numberOfAttack;
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("attack"+tempAttckNum);
            // if (currentWeaponConfig.hasProjectile())
            // {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //     RaycastHit hit;
            //     Physics.Raycast(ray, out hit, 30f);
            //     Vector3 targetPos = hit.point;
            //     targetPos.y = transform.position.y;
            //     transform.LookAt(targetPos);
            //     currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, baseStat.GetStat(Stat.Damage));
            // }
            if(attackTime>0){
                isAttacking=true;
                yield return new WaitForSeconds(attackTime);
                isAttacking=false;
            }
            anim.ResetTrigger("attack"+tempAttckNum);
            anim.SetTrigger("stopAttack");
        }
        
        // public void Attack(GameObject combatTarget)
        // {
        //     attackTime = Util.GetCurrentAnimationTime(attackTime,"attack",anim);
        //     target = combatTarget.GetComponent<Health>();
        //     mover.Cancel();
        //     GetComponent<ActionScheduler>().StartAction(this);
        // }

        public void Attack(GameObject combatTarget)
        {
            float staminaToConsume = currentWeaponConfig.GetStaminaPerHit();
            attackTime = Util.GetCurrentAnimationTime(attackTime,"attack",anim)/2;
            if(combatTarget!=null){
                //for AiController
                mover.Cancel();
                target = combatTarget.GetComponent<Health>();
                GetComponent<ActionScheduler>().StartAction(this);
            }else{
                //for PlayerController
                AttackBehaviour(staminaToConsume);
            }
        }

        public void SpecialAttack()
        {
            attackTime = Util.GetCurrentAnimationTime(attackTime,"attack",anim)/4*2;
            StartCoroutine("TriggerSpecialAttack");
            stamina.ConsumeStaminaOnce(currentWeaponConfig.GetStaminaSpecialAttack(),Stamina.StaminaType.attack);
            timeSinceAttack = 0f;
        }

        // private void TriggerAttack()
        // {
        //     anim.ResetTrigger("stopAttack");
        //     anim.SetTrigger("attack");
        // }

        // private void OnCollisionEnter(Collision other) {
        //     print("collision enter: " + other.gameObject.name);    
        // }

        private void OnTriggerEnter(Collider other)
        {
            if (this.gameObject.tag.Equals("Player"))
            {
                HandlePlayerHit(other);
            }
            else
            {
                // if(other.gameObject.name=="Player"){
                //     print("target: " + target.gameObject.name);
                //     print("other: " + other.gameObject.name);
                //     print("can trigger: " + currentWeapon.value.GetCanTrigger());
                // }
                if (target == null) return;
                if (!currentWeapon.value.GetCanTrigger()) return;
                if (target != other.gameObject.GetComponent<Health>()) return;
                if (target.GetInstigatorDamageCooldown(character) > 0) return;
                float damage = baseStat.GetStat(Stat.Damage);
                Armour armour = other.gameObject.GetComponent<Armour>();

                if(target.IsDamageTaken(character, damage)){
                    if (currentWeapon.value != null)
                    {
                        currentWeapon.value.OnHit(armour.GetArmourType());
                    }
                }

                // if (currentWeaponConfig.hasProjectile())
                // {
                //     currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
                // }
                // else
                // {
                //     target.TakeDamage(gameObject, damage);
                // }
            }
        }

        private void HandlePlayerHit(Collider other) {
            bool canTrigger = currentWeapon.value.GetCanTrigger();
            if(!canTrigger){
                return;
            }
            // print("trigger");
            // print(other.gameObject.name);
            Health target = other.gameObject.GetComponent<Health>();
            Armour armour = other.gameObject.GetComponent<Armour>();
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            // if(target.IsDamageTaken(GameObject.FindGameObjectWithTag("Player"), damage)){
            //     currentWeapon.value.OnHit(armour.GetArmourType());
            // }
            if ((currentWeapon.value != null && armour!=null && target==null)
                || target.IsDamageTaken(character, damage))
            {
                currentWeapon.value.OnHit(armour.GetArmourType());
            }
            Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
            if(obstacle!=null&&obstacle.IsRecoil()){
                //trigger obstacle animation
                anim.SetBool("recoiling",true);
                //stop player input in animation?
            }
            if(target == null ) return;
            // float damage = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().GetStat(Stat.Damage);
            // if(target.IsDamageTaken(GameObject.FindGameObjectWithTag("Player"), damage)){
            //     currentWeapon.value.OnHit(armour.GetArmourType());
            // }
        }

        // //Animation effect
        void Hit()
        {
        //     if (target == null) return;
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
        }

        public void OnAttack(){
            currentWeapon.value.OnAttack();
        }

        public void Shoot()
        {
            if(!currentWeaponConfig.hasProjectile()) return;
            currentWeapon.value.OnAttack();
            currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, character, baseStat.GetStat(Stat.Damage),shootDirection);
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetRange(); ;
        }


        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) {return false;}
            if (combatTarget == this.gameObject) {return false;}
            if (!GetIsInRange(combatTarget.transform)) return false;
            if (!mover.CanMoveTo(combatTarget.transform.position)){ 
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null & !targetToTest.IsDead();
        }

        public void Cancel()
        {
            StopAttack();
            // anim.enabled=false;
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            anim.ResetTrigger("attack1");
            anim.ResetTrigger("attack2");
            anim.ResetTrigger("attack3");
            anim.SetTrigger("stopAttack");
            // anim.ResetTrigger("stopAttack");
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
            if(weapon.IsDual()){
                currentSubWeapon.value = weapon.GetSubWeapon();
            }
            if(GetComponent<IKController>()!= null){
                GetComponent<IKController>().SetGrabObj(currentWeapon.value.GetGrabObj());
            }
        }

        private void UpdateWeapon(){
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if(weapon == null) {
                weapon = defaultWeapon;
            }
            EquipWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            // if(gameObject.tag=="Player"&&equipment.GetItemInSlot(EquipLocation.Weapon)==null){
            //     equipment.AddItem(EquipLocation.Weapon,weaponConfig);
            // }
            Animator animator = anim;
            Weapon weapon = weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
            return weapon;
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

        // public void EnableTrigger(){
        //     currentWeaponConfig.EnableTrigger();
        // }

        // public void DisableTrigger(){
        //     currentWeaponConfig.DisableTrigger();
        // }

        public void StartAttack(){
            currentWeapon.value.OnStartAttack();
        }

        public int GetNumberOfAttack(){
            return numberOfAttack;
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

        public void EnableSubWeaponTrigger(){
            // currentSubWeapon.value.EnableTrigger();
            currentWeapon.value.EnableTrigger();
            // agent.SetDestination(transform.position);
            // agent.enabled = false;
        }

        public void DisableSubWeaponTrigger(){
            // currentSubWeapon.value.DisableTrigger();
            currentWeapon.value.DisableTrigger();
            // agent.enabled = true;
        }

        public float GetBlockingAngle(){
            return currentWeaponConfig.GetBlockingAngle();
        }

        public void PlayEffect(){
            currentWeapon.value.PlayEffect();
        }

        public void EnableTrail(){
            currentWeapon.value.EnableTrail();
        }

        public void DisableTrail(){
            currentWeapon.value.DisableTrail();
        }

        public bool GetIsAttacking(){
            return isAttacking;
        }

        public void IncreaseMaxNumberOfAttack(int add){
            Mathf.Clamp(tempMaxNumberOfAttack+add,1,maxNumberOfAttack);
        }

        public bool HasProjectile(){
            return currentWeaponConfig.hasProjectile();
        }

        public void SetShootDirection(Vector3 direction){
            direction.y = transform.position.y;
            this.shootDirection = direction;
        }
    }
}