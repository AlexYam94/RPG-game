using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public void Save(string saveFile){
            string path = GetPathFromSaveFile(saveFile);
            print("saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {  

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, CaptureState());

            }
        }

        public void Load(string saveFile){
            string path = GetPathFromSaveFile(saveFile);
            print("loading from " + path);
            using (FileStream stream = File.Open(path, FileMode.Open))
            {  
                // byte[] buffer = new byte[stream.Length];
                // stream.Read(buffer,0,buffer.Length);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                RestoreState(binaryFormatter.Deserialize(stream));
            }
        }

        private object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            return state;
        }

        private void RestoreState(object state)
        {
            Dictionary<string, object> dict = (Dictionary<string,object>) state;
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                saveable.RestoreState(dict[saveable.GetUniqueIdentifier()]);
            }
        }

        public string GetPathFromSaveFile(string saveFile){
            return Path.Combine(Application.persistentDataPath,saveFile)+".sav";
        }

        
    }
}