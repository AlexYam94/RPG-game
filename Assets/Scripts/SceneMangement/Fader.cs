using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField]
        private int time;

        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;

        // private int deltaTime = time/Time.deltaTime;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            // StartCoroutine(FadeOutIn());
        }

        public void FadeOutImmediate(){
            canvasGroup.alpha = 1;
        }

        public Coroutine Fade(float target, float time){
            if(currentActiveFade !=null){
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(0f,time));
            return currentActiveFade;
        }

        public Coroutine FadeOut(float time){
            return Fade(1,time);
        }

        public Coroutine FadeIn(float time){
            return Fade(0,time);
        }

        private IEnumerator FadeRoutine(float target, float time){
            //number of frames = time/Time.deltaTime
            //deltaAlpha = 1/number of frames
            //           = Time.deltaTime/time
            while(Mathf.Approximately(canvasGroup.alpha, target)){ //alpha is not 1
                //moving alpha toward 1
                canvasGroup.alpha=Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime/time);
                //ask coroutine to wait for 1 frame
                yield return null;
            }
        }


    }
}