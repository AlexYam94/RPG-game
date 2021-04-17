using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Control
{

    public class ButtonPickup : MonoBehaviour
    {
        [SerializeField] GameObject uiContainer = null;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                uiContainer.SetActive(true);
                //register this pickup uid? to pickup manager
                PickupManager.Register(GetComponent<Pickup>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                uiContainer.SetActive(false);
                //deregister this pickup uid? to pickup manager
                PickupManager.Deregister(GetComponent<Pickup>());
            }
        }
    }

}