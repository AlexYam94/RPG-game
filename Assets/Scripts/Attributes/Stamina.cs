using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Stats;
using UnityEngine;
namespace RPG.Attributes
{
    public class Stamina : MonoBehaviour
    {
        [SerializeField] float staminaRegen = .1f;
        [SerializeField] float staminaRegenRate = .01f;
        [SerializeField] float staminaRegenInterval = 0f;
        [SerializeField] float sprintStaminaInterval = 1f;
        [SerializeField] float attckStaminaInterval = 1.5f;
        [SerializeField] float blockStaminaInterval = 1.5f;
        [SerializeField] float rollStaminaInterval = 1f;

        LazyValue<float> staminaPoints;
        float sinceLastAction = 0f;

        public enum StaminaType{
            sprint,
            attack,
            block,
            roll
        }

        // Start is called before the first frame update
        void Awake()
        {
            staminaPoints = new LazyValue<float>(GetInitialStamina);
        }
        
        private void Start() {
            StartCoroutine("RegenerateStamina");
            // InvokeRepeating("RegenerateStamina",0.0f, staminaRegenInterval);
        }

        private float GetInitialStamina(){
            return GetComponent<BaseStats>().GetStat(Stat.Stamina);
        }

        // Update is called once per frame
        void Update()
        {
            if(staminaRegenInterval > 0){
                staminaRegenInterval = Mathf.Max(staminaRegenInterval - Time.deltaTime,0);
            }
        }

        void FixedUpdate() {
        }

        public void ConsumeStaminaOnce(float stamina, StaminaType type){
            lock(staminaPoints){
                staminaRegenInterval = FindIntervalByType(type);
                // print("stamina to reduce: " + stamina);
                // print("stamina before: "  + staminaPoints.value);
                staminaPoints.value -= stamina;
                // print("stamina after: " + staminaPoints.value);
            }
        }

        private float FindIntervalByType(StaminaType type)
        {
            switch (type)
            {
                case StaminaType.roll:
                    return rollStaminaInterval;
                case StaminaType.attack:
                    return attckStaminaInterval;
                case StaminaType.sprint:
                    return sprintStaminaInterval;
                case StaminaType.block:
                    return blockStaminaInterval;
                
            }
            return 0;
            
        }

        public IEnumerator ConsumeStaminaCon(float stamina, StaminaType type){
            ConsumeStaminaOnce(stamina,type);
            yield return new WaitForSeconds(.3f);
        }

        public float GetFraction(){
            lock(staminaPoints){
                return staminaPoints.value/GetComponent<BaseStats>().GetStat(Stat.Stamina); 
            }
        }

        public float GetPercentage(){
            lock(staminaPoints){
                return staminaPoints.value/GetComponent<BaseStats>().GetStat(Stat.Stamina) * 100;
            }
        }

        public IEnumerator RegenerateStamina(){
            // if(staminaPoints.value < GetComponent<BaseStats>().GetStat(Stat.Stamina))
            //     staminaPoints.value += staminaRegen;
            
            // float maxStamina = GetComponent<BaseStats>().GetStat(Stat.Stamina);
            while(true){
                lock(staminaPoints){
                    if(staminaRegenInterval<=0 && staminaPoints.value < GetComponent<BaseStats>().GetStat(Stat.Stamina))
                        staminaPoints.value += staminaRegen;
                }
                yield return new WaitForSeconds(staminaRegenRate);
            }
            // yield return new WaitForSeconds(staminaRegenInterval);
        }

        public bool HasStaminaLeft(){
            lock(staminaPoints){
                return staminaPoints.value > 0;
            }
        }
    }
}