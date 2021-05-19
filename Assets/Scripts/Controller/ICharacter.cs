
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Core
{
    public interface ICharacter : IComponent
    {

        Health GetHealthComponent();

        Fighter GetFighterComponent();

        void HandleMovement();

        void HandleCombat();

        void HandleRotation();

        void InteractWithComponent();

        GameObject GetGameObject();

    }
}