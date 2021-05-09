using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] bool isRecoil;

        public bool IsRecoil(){
            return isRecoil;
        }
    }
}