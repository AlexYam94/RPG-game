using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;

namespace RPG.Control
{

    public class PickupManager : MonoBehaviour
    {
        public static PickupManager instance;

        private static List<Pickup> pickups = new List<Pickup>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = new PickupManager();
            }
        }

        public static void Register(Pickup pickup)
        {
            pickups.Add(pickup);
        }

        public static void Deregister(Pickup pickup)
        {
            pickups.Remove(pickup);
        }

        public static void Pickup(){
            if(pickups.Count==0||pickups[0]==null) return;
            pickups[0].PickupItem();
            Deregister(pickups[0]);
        }
    }
}
