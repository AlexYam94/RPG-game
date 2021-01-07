using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBarDisplay : MonoBehaviour
    {

        Health health;
        RectTransform rt;
        float barWidth = 250;
        Slider slider = null;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            slider = GetComponent<Slider>();
        }

        // Start is called before the first frame update
        void Start()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            barWidth = rt.sizeDelta.x;
        }

        void Update(){
            // Vector2 size = rt.sizeDelta;
            // size.x = barWidth * health.GetPercentage()/100;
            // rt.sizeDelta = size;
            slider.value = slider.maxValue * health.GetFraction();
        }
    }
}