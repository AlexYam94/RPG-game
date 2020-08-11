using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,100)]
        [SerializeField] int startinglevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        Experience experience;
        LazyValue<int> currentLevel;
        public event Action onLevelUp;

        private void Awake() {
            currentLevel = new LazyValue<int>(CalculateLevel);
            experience = GetComponent<Experience>();
        }

        private void OnEnable() {
            if(experience != null){
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if(experience != null){
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                onLevelUp();
                LevelUpEffect();
            }
        }

        private void LevelUpEffect()
        {
            if (levelUpParticleEffect != null)
            {
                GameObject.Instantiate(levelUpParticleEffect, transform);
            }
        }

        public float GetStat(Stat stat){
            
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1+GetPercentageModifier(stat)/100);
        }

        public float GetBaseStat(Stat stat){
            return progression.GetStat(stat,characterClass,GetLevel());
        }

        private float GetPercentageModifier(Stat stat){
            if(!shouldUseModifiers) return 0;
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>()){
                foreach(float modifier in provider.GetPercentageModifiers(stat)){
                    total += modifier;
                }
            }
            return total;
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            if(!shouldUseModifiers) return 0;
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>()){
                foreach(float modifier in provider.GetAdditiveModifiers(stat)){
                    total += modifier;
                }
            }
            return total;
        }

        public int GetLevel(){
            return currentLevel.value;
        }

        public int CalculateLevel(){
            Experience exp = GetComponent<Experience>();
            if(exp == null) return startinglevel;

            float currentExp = exp.GetCurrentExpPoint();
            float penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level < penultimateLevel; level++){
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp,characterClass, level);
                if(xpToLevelUp > currentExp){
                    return level;
                }
            }
            return (int)penultimateLevel + 1;
        }

        public float GetExpToLevelUp(){
            return progression.GetStat(Stat.ExperienceToLevelUp,characterClass, GetLevel());
        }

    }
}
