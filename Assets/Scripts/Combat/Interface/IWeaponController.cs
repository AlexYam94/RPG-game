using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponController 
{
    Weapon EquipWeapon(IWeaponConfig weapon);
}
