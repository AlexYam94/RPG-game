using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField]
        int sceneToLoad = -1;

        [SerializeField]
        Transform spawnPoint;

        [SerializeField]
        DestinationIdentifier destination;

        [SerializeField]
        float fadeOutTime = 2f;

        [SerializeField]
        float fadeInTime = 2f;

        [SerializeField]
        float fadeWaitTime = 1f;

        PlayerController player = null;

        private void Start()
        { 
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // if(other.gameObject.tag=="Player"){
            //     print("trigger portal");
            //     SceneManager.LoadScene(sceneToLoad);
            // }
            if (other.tag == "Player")
                StartCoroutine(Transition(other));
        }

        private IEnumerator Transition(Collider other)
        {
            if(sceneToLoad < 0){
                Debug.LogError("Scene to load not set");
                yield break;
            }
            player.enabled = false;
            Fader fader = FindObjectOfType<Fader>();
            
            yield return fader.FadeOut(fadeOutTime);
            DontDestroyOnLoad(this.gameObject);

            SavingWrapper wrapper = GameObject.FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            PlayerController newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();

            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;

            Destroy(this.gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            // player.GetComponent<NavMeshAgent>().enabled = false;
            // player.transform.position = otherPortal.spawnPoint.position;
            // player.transform.rotation = otherPortal.spawnPoint.rotation;
            // player.GetComponent<NavMeshAgent>().enabled = true;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal != this&&portal.destination==this.destination)
                    return portal;
            }
            return null;
        }
    }

}