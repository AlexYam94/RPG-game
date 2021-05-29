using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class EnemyHealthBar : EnemyBar
    {

        // Update is called once per frame
        void Update()
        {
            Follow();
            UpdateUI();
        }
    }
}