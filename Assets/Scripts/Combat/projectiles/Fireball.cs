using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{

    public class Fireball : MonoBehaviour
    {
        [SerializeField] float totalDamage = 3;
        [SerializeField] float duration = 3;

        private Health target = null;
        GameObject instigator = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            SetTarget();
        }

        public void SetTarget()
        {
            this.target = GetComponent<Projectile>().GetTarget();
            this.instigator = GetComponent<Projectile>().GetInstigator();
        }

        public void Burn()
        {
            if (target == null) return;
            target.StartDamageOfTime(totalDamage, duration, instigator);
        }
    }

}