using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{


    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        DamageText damageTextPrefab=null;

        public void Spawn(float damage){
            print("damage text spawn");
            if(damageTextPrefab==null) return;
            DamageText instance =  Instantiate<DamageText>(damageTextPrefab,transform);
            instance.SetValue(damage);
        }
    }
}