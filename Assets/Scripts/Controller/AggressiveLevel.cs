using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "AggressiveLevel", menuName = "Stat/NewAggressiveLevel", order = 0)]
    public class AggressiveLevel : ScriptableObject
    {
        [SerializeField] AggressiveLevelClass[] aggressiveLevelClasses = null;

        Dictionary<AggressiveLevelEnum,float> lookupTable;

        public enum AggressiveLevelEnum{
            Level1,
            Level2,
            Level3,
            Level4,
            Level5
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<AggressiveLevelEnum,float>();

            foreach (AggressiveLevelClass aggressiveLevelClass in aggressiveLevelClasses)
            {
                lookupTable.Add(aggressiveLevelClass.level, aggressiveLevelClass.percentage);
            }

        }
        
        public float lookUp(AggressiveLevelEnum level){
            BuildLookup();
            return lookupTable[level];
        }
        
        [System.Serializable]
        public class AggressiveLevelClass {
            public AggressiveLevelEnum level;
            public float percentage;
        }

        
    }
}
