using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.AI;

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

        private void Start()
        {
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
            DontDestroyOnLoad(this.gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            Portal otherPortal = GetOtherPortal();

            UpdatePlayer(otherPortal);

            Destroy(this.gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            // player.GetComponent<NavMeshAgent>().enabled = false;
            // player.transform.position = otherPortal.spawnPoint.position;
            // player.transform.rotation = otherPortal.spawnPoint.rotation;
            // player.GetComponent<NavMeshAgent>().enabled = true;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
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