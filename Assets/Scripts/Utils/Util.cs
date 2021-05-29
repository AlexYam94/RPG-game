using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

namespace RPG.Tool
{

    public class Util : MonoBehaviour
    {   
        public static float GetCurrentAnimationTime(float originTime, String nameContained, Animator anim)
        {
            AnimatorClipInfo[] animatorClipInfos = anim.GetCurrentAnimatorClipInfo(0);
            
            // result = animatorClipInfos[0].clip.length;
            foreach (AnimatorClipInfo clipInfo in animatorClipInfos){
                if(clipInfo.clip.name.IndexOf(nameContained, StringComparison.InvariantCultureIgnoreCase)>=0){
                        // print(clip.name + ", " +clip.length);
                        Debug.Log(clipInfo.clip.name);
                        return clipInfo.clip.length;
                    }
                }
            // AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            // foreach (AnimationClip clip in clips){
            //     if(clip.name.IndexOf(nameContained, StringComparison.InvariantCultureIgnoreCase)>=0){
            //         // print(clip.name + ", " +clip.length);
            //         result = clip.length;
            //     }
            // }
            return 0;
        } 

        public static float GetBlockingAngle(GameObject obj){
            return obj.GetComponent<Fighter>().GetBlockingAngle();
        }

        
    }

}

