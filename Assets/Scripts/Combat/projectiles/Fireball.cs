using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{

    public class Fireball : Projectile
    {
        [SerializeField] float totalDamage = 3;
        [SerializeField] float duration = 3;

        // void Update()
        // {
        //     SetTarget();
        // }

        // public void SetTarget()
        // {
        //     this.target = GetComponent<Projectile>().GetTarget();
        //     this.instigator = GetComponent<Projectile>().GetInstigator();
        // }

        public override void SpecialDamage()
        {
            if (target == null) return;
            target.StartDamageOfTime(totalDamage, duration, instigator);
        }
    }

}