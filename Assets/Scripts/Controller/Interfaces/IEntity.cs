
using System;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    public interface IEntity{
        void FromToWhere(IState from, IState to, Func<bool> where);
    }
}