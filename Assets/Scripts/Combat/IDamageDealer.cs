using UnityEngine;
using UnityEngine.Events;
using RPG.Core;

namespace RPG.Combat
{
    public interface IDamageDealer 
    {
        [SerializeField] UnityEvent onHit{get;}
        [SerializeField] UnityEvent onAttack{get;}
        [SerializeField] UnityEvent onStartAttack{get;}
        float damage {get; set; }
        ICharacter instigator {get;set;}
        void DealDamage(IDamageReceiver target);
    }    
}