using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
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

        public object CaptureState()
        {
            return isTriggered;
        }

        public void RestoreState(object state)
        {
            isTriggered = (bool)state;
        }
    }
}

