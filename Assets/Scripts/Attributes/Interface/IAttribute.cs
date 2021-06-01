using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public interface IAttribute
    {
        float GetFraction();
        float GetPercentage();
        bool isEnable();
    }
}