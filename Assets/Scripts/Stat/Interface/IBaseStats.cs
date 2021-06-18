using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public interface IBaseStats
    {
        event Action onLevelUp;

        float GetStat(Stat stat);

        float GetBaseStat(Stat stat);

        int GetLevel();

        int CalculateLevel();

        float GetExpToLevelUp();
    }
}
