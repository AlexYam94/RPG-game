using RPG.Core;

namespace RPG.Attributes{
    public interface IHealth{
        bool IsDamageTaken(ICharacter instigator, float damage);
        void ApplyDamage(float damage);
    }
}