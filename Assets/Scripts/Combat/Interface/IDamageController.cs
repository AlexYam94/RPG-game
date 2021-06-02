using RPG.Core;

namespace RPG.Combat{
    public interface IDamageController{
        bool CanTakeDamageFromGambObject(ICharacter instigator);
        void ApplyDamage(ICharacter instigator, float damage);
    }
}