using System.Collections;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField]
        float fadeInTime = 2f;

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            // Fader fader = FindObjectOfType<Fader>();
            // fader.FadeOutImmediate();
            // yield return fader.FadeIn(2f);
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.F5))
            {
                Save();
            }
            else if(Input.GetKeyDown(KeyCode.F9))
            {
                Fader fader = FindObjectOfType<Fader>();
                fader.FadeOutImmediate();
                Load();
                fader.Fade(0,fadeInTime);
            }
            else if(Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }


        }

        public void Load()
        {
            
           StartCoroutine(GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile));
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete(){
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
