using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class DamageReceiveObject
    {
        IDamageController _controller;
        float _partDamageMultiplier;
        
        public DamageReceiveObject(IDamageController controller, float partDamageMultiplier)
        {
            _controller = controller;
            _partDamageMultiplier = partDamageMultiplier;
        }

        public void TakeDamage(ICharacter instigator, float damage)
        {
            if (_controller.CanTakeDamageFromGambObject(instigator))
                _controller.ApplyDamage(instigator, CalculateDamage(damage));
        }

        public float CalculateDamage(float damage)
        {
            return damage * GetDefenseStat() * GetArmourStat() * _partDamageMultiplier;
        }

        private float GetDefenseStat()
        {

            //TODO
            return 1f;
        }

        private float GetArmourStat()
        {
            //TODO
            return 1f;
        }
    }
}