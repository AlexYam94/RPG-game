using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool isTriggered = false;
        // Start is called before the first frame update
        private void OnTriggerEnter(Collider other)
        {
            if (!isTriggered && other.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                isTriggered = true;
            }
        }
    }
}

