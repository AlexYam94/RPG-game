using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class Footstep : MonoBehaviour
    {
        [SerializeField] FootstepType footstepType;
        public FootstepType GetFootstepType(){
            return footstepType;
        }
    }

}
