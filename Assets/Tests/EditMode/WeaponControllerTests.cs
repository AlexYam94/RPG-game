using GameDevTV.Inventories;
using GameDevTV.Utils;
using NSubstitute;
using NUnit.Framework;
using RPG.Combat;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class WeaponControllerTests
    {
        Transform left;
        Transform right;
        Animator anim;
        Equipment equipment;
        IWeaponConfig weaponConfig;
        ICharacter instigator;
        Weapon weapon;
        WeaponControllerBehaviour behaviour;
        LazyValue<Weapon> currentWeapon;

        [OneTimeSetUp]
        public void init()
        {
            left = Substitute.For<Transform>();
            right = Substitute.For<Transform>();
            anim = Substitute.For<Animator>();
            equipment = Substitute.For<Equipment>();
            weaponConfig = Substitute.For<IWeaponConfig>();
            instigator = Substitute.For<ICharacter>();
            weapon = Substitute.For<Weapon>();
            currentWeapon = new LazyValue<Weapon>(() => weapon);
            weaponConfig.Spawn(left,right,anim,instigator).Returns(weapon);
            behaviour = new WeaponControllerBehaviour(right, left, anim, equipment, weaponConfig, instigator);
            behaviour.init();
        }

        [Test]
        public void TestAttachWeaponPass() {
            Assert.IsInstanceOf<Weapon>(behaviour.AttachWeapon(weaponConfig, instigator));
        }

        [Test]
        public void TestWeaponHasProjectile()
        {
            weaponConfig.HasProjectile().Returns(true);
            Assert.IsTrue(weaponConfig.HasProjectile());
        }

        [Test]
        public void TestEquipWeapon()
        {
            IWeaponConfig weaponConfig2 = Substitute.For<IWeaponConfig>();
            behaviour.AttachWeapon(weaponConfig2, instigator).Returns(weapon);
            Assert.IsInstanceOf<Weapon>(behaviour.EquipWeapon(weaponConfig2));
        }

    }
}