using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stat/NewProgression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            // foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            // {
            //     if(progressionCharacterClass.characterClass != characterClass ) continue;
            //         // return progressionCharacterClass.health[level-1];
            //     foreach(ProgressionStat progressionStat in progressionCharacterClass.stats){
            //         if(progressionStat.stat != stat) continue;
            //         if(progressionStat.levels.Length < level) continue;
            //         return progressionStat.levels[level-1];
            //     }
            // }
            // return 30;
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            if (levels.Length < level) return 0;
            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
                {
                    statLookupTable.Add(progressionStat.stat, progressionStat.levels);
                }
                lookupTable.Add(progressionCharacterClass.characterClass, statLookupTable);
                statLookupTable = new Dictionary<Stat, float[]>();
            }

        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            //public is automatically serializedField
            public CharacterClass characterClass;
            // public float[] health;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}