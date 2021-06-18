using GameDevTV.Inventories;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    public class WeaponController : MonoBehaviour, IWeaponController, ISaveable
    {
        //WeaponController responsibility:
        //1. Store main weapon
        //2. Store sub weapon
        //3. attach weapon to right hand or left hand
        //4. register this gameObject to weapon
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";

        WeaponControllerBehaviour behaivour;

        private void Awake()
        {
            behaivour = new WeaponControllerBehaviour(rightHandTransform,leftHandTransform, GetComponent<Animator>(), GetComponent<Equipment>(), defaultWeapon,GetComponent<ICharacter>()) ;
        }
        void Start()
        {
            behaivour.init();

        }
        public Weapon EquipWeapon(IWeaponConfig weapon)
        {
            return behaivour.EquipWeapon(weapon);
        }
        public bool HasProjectile(){
            return behaivour.HasProjectile();
        }
        public object CaptureState()
        {
            throw new System.NotImplementedException();
        }

        public void RestoreState(object state)
        {
            throw new System.NotImplementedException();
        }

        #region animation_events
        public void PlayEffect(){ behaivour._currentWeapon.value.PlayEffect();}
        public void EnableTrail(){ behaivour._currentWeapon.value.EnableTrail();}
        public void DisableTrail(){ behaivour._currentWeapon.value.DisableTrail();}
        public void EnableWeaponTrigger(){ behaivour._currentWeapon.value.EnableTrigger();}
        public void DisableWeaponTrigger(){ behaivour._currentWeapon.value.DisableTrigger();}
        public void EnableSubWeaponTrigger(){ behaivour._currentWeapon.value.EnableTrigger();}
        public void DisableSubWeaponTrigger(){ behaivour._currentWeapon.value.DisableTrigger();}

        #endregion
    }
}