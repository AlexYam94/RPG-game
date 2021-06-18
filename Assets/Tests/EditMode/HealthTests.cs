using NSubstitute;
using NUnit.Framework;
using RPG.Attributes;
using RPG.Core;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class HealthTests
    {
        HealthControllerBehaviour behaviour;
        Animator anim;
        ActionScheduler actionScheduler;
        IBaseStats stat;

        [OneTimeSetUp]
        public void init()
        {
            anim = Substitute.For<Animator>();
            actionScheduler = Substitute.For<ActionScheduler>();
            stat = Substitute.For<IBaseStats>();
            stat.GetStat(Stat.Health).Returns(100);
            behaviour = new HealthControllerBehaviour(0f, anim, actionScheduler, null, null, stat);
            behaviour.init();
            //behaviour.healthPoints.value.Returns(100);
        }

        [Test]
        public void TestHealthApplyDamageTest()
        {
            float hp = behaviour.healthPoints.value;
            float damage = 10f;
            behaviour.ApplyDamage(damage);
            hp -= damage;
            Assert.AreEqual(behaviour.healthPoints.value, hp);
        }

        [Test]
        public void TestHealthDeathTest()
        {
            float hp = behaviour.healthPoints.value;
            //health.ApplyDamage(hp);
            behaviour.ApplyDamage(hp);
            behaviour.CheckDeath();
            Assert.AreEqual(behaviour.IsDead(), true);

        }
    }
}