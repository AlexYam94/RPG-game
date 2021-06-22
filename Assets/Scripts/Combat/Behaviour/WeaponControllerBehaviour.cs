using GameDevTV.Inventories;
using GameDevTV.Utils;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponControllerBehaviour
    {
        public LazyValue<Weapon> _currentWeapon { get; private set; }

        Transform _rightHandTransform;
        Transform _leftHandTransform;
        Animator _anim;
        Equipment _equipment;
        IWeaponConfig _defaultWeapon;
        IWeaponConfig _currentWeaponConfig = null;
        LazyValue<Weapon> _currentSubWeapon;
        IKController _iKController;
        ICharacter _instigator;
        public WeaponControllerBehaviour(Transform rightHandTransform, Transform leftHandTransform, Animator anim, Equipment equipment, IWeaponConfig defaultWeapon, ICharacter instigator)
        {
            _rightHandTransform = rightHandTransform;
            _leftHandTransform = leftHandTransform;
            _anim = anim;
            _equipment = equipment;
            _defaultWeapon = defaultWeapon as WeaponConfig;
            _instigator = instigator;
        }

        #region private
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon, _instigator);
        }
        #endregion

        #region public

        public void init()
        {
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        public Weapon AttachWeapon(IWeaponConfig weaponConfig, ICharacter instigator)
        {
            Weapon weapon = weaponConfig.Spawn(_rightHandTransform, _leftHandTransform, _anim, instigator);
            return weapon;
        }

        public void UpdateWeapon()
        {
            IWeaponConfig weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                weapon = _defaultWeapon;
            }
            EquipWeapon(weapon);
        }

        public Weapon EquipWeapon(IWeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            _currentWeapon.value = AttachWeapon(weapon, _instigator);
            if (weapon.IsDual())
            {
                _currentSubWeapon.value = weapon.GetSubWeapon();
            }
            if (_iKController != null)
            {
                _iKController.SetGrabObj(_currentWeapon.value.GetGrabObj());
            }
            return _currentWeapon.value;
        }
        public bool HasProjectile()
        {
            return _currentWeaponConfig.HasProjectile();
        }
        #endregion
    }
}
