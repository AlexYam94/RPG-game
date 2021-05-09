using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Movement
{
    public class FootstepController : SerializedMonoBehaviour
    {
        [SerializeField] Dictionary<FootstepType, AudioClip[]> footstepTable;
        [SerializeField] AudioSource leftFootstepSource;
        [SerializeField] AudioSource rightFootstepSource;

        private FootstepType currentFootStepType = FootstepType.defaultFootstep;

        public void FootL()
        {
            PlayFootStep(leftFootstepSource);
        }

        public void FootR(){
            PlayFootStep(rightFootstepSource);
        }

        private void OnTriggerEnter(Collider other) {
            ChangeFootstepType(other);
        }

        private void OnTriggerStay(Collider other) {
            ChangeFootstepType(other);
        }

        private void OnTriggerExit(Collider other) {
            Footstep footstep = other.gameObject.GetComponent<Footstep>();
            if(footstep){
                currentFootStepType = FootstepType.defaultFootstep;
            }
        }

        private void ChangeFootstepType(Collider other){
            Footstep footstep = other.gameObject.GetComponent<Footstep>();
            if(footstep){
                currentFootStepType = footstep.GetFootstepType();
            }
        }

        private void PlayFootStep(AudioSource footstepSource)
        {
            // if(footstepSource.isPlaying) return;
            if (footstepTable.ContainsKey(currentFootStepType))
            {
                AudioClip[] sources = footstepTable[currentFootStepType];
                System.Random rnd = new System.Random();
                footstepSource.PlayOneShot(sources[rnd.Next(0, sources.Length - 1)]);
            }
        }

    }
}
