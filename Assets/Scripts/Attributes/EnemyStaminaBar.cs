using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class EnemyStaminaBar : EnemyBar
    {
        // Update is called once per frame
        void Update()
        {
            Follow();
            UpdateUI();
        }
        
        protected override IAttribute GetAttriute()
        {
            return transform.parent.GetComponentInChildren(typeof(Stamina)) as IAttribute;
        }
    }
}