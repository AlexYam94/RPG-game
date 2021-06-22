using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Factory {
    public class AttackControllerFactory : Singleton<AttackControllerFactory>
    {
    public IAttackControllerBehaviour create(AttackControllerType type)
        {
            IAttackControllerBehaviour behaviour = null;
            switch (type)
            {
                case AttackControllerType.player:
                    behaviour = new PlayerAttackControllerBehaviour();
                    break;
                case AttackControllerType.enemy:
                    behaviour = new EnemyAttackControllerBehaviour();
                    break;
            }
            return behaviour;
        }
    }
}
