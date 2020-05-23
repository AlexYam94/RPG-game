using System;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject persisitentObjectPrefab;


        static bool hasSpawne = false;

        private void Awake() {
            if (hasSpawne) return;

            hasSpawne = true;

            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = GameObject.Instantiate(persisitentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}