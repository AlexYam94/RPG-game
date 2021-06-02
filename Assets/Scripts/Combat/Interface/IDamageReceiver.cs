using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using UnityEngine.Events;
using RPG.Core;

namespace RPG.Combat
{
    public interface IDamageReceiver 
    {
        [SerializeField] UnityEvent onGetHit{get;}
        void TakeDamage(ICharacter instigator, float damage);
        
    }    
}