using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {

        [SerializeField]
        string uniqueIdentifier = "";

        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            if (string.IsNullOrEmpty(property.stringValue) || !isUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
#endif

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
            // return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                saveable.RestoreState(dict[saveable.GetType().ToString()]);
            }
            // GetComponent<NavMeshAgent>().enabled = false;
            // transform.position = ((SerializableVector3)state).ToVector();
            // GetComponent<NavMeshAgent>().enabled = true;
            // GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private bool isUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate))
                return true;

            if (globalLookup[candidate] == this)
                return true;

            if (globalLookup[candidate] == null || globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
            }
            return false;
        }
    }

}