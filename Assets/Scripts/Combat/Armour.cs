using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{

    public class Armour : MonoBehaviour
    {
        [SerializeField] ArmourType armourType;

        public ArmourType GetArmourType(){
            return armourType;
        }
    }
}
