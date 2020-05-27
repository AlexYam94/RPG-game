using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField]
        private int time;

        CanvasGroup canvasGroup;

        // private int deltaTime = time/Time.deltaTime;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            // StartCoroutine(FadeOutIn());
        }

        public void FadeOutImmediate(){
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time){
            //number of frames = time/Time.deltaTime
            //deltaAlpha = 1/number of frames
            //           = Time.deltaTime/time
            while(canvasGroup.alpha<1){ //alpha is not 1
                //moving alpha toward 1
                canvasGroup.alpha+=Time.deltaTime/time;
                //ask coroutine to wait for 1 frame
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time){
            //number of frames = time/Time.deltaTime
            //deltaAlpha = 1/number of frames
            //           = Time.deltaTime/time
            while(canvasGroup.alpha>0){ //alpha is not 1
                //moving alpha toward 1
                canvasGroup.alpha-=Time.deltaTime/time;
                //ask coroutine to wait for 1 frame
                yield return null;
            }
        }
    }
}