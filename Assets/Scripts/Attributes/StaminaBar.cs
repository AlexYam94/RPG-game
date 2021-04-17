using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class StaminaBar : MonoBehaviour
    {
        [SerializeField] Stamina staminaComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;
        // Update is called once per frame
        void Update()
        {
            if(foreground!=null){
                float fraction = staminaComponent.GetFraction();
                foreground.localScale = new Vector3(fraction,1,1);
                if(Mathf.Approximately(fraction,0)||Mathf.Approximately(fraction,1)){
                    canvas.enabled = false;
                    return;
                }else{
                    canvas.enabled = true;
                }
                // foreground.localScale = new Vector3(fraction,1,1);
            }
        }
    }
}