using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField]
        float fadeInTime = 2f;

        private IEnumerator Start() {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(2f);
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            else if(Input.GetKeyDown(KeyCode.L))
            {
                Fader fader = FindObjectOfType<Fader>();
                fader.FadeOutImmediate();
                Load();
                StartCoroutine(fader.FadeIn(fadeInTime));
            }


        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
    }
}
