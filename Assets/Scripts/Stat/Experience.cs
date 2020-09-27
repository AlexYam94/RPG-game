using RPG.Saving;
using UnityEngine;
using RPG.Attributes;
using System;

namespace RPG.Stats
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public event Action onExperienceGained;

        public void Start(){
        }


        public void GainExperince(float experience){
            experiencePoints += experience;
            GameObject.Find("Exp Bar").GetComponent<ExpBarDisplay>().UpdateExpBar(experience);
            onExperienceGained();
        }

        public float GetCurrentExpPoint(){
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}