using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBarDisplay : MonoBehaviour
    {

        Health health;
        RectTransform rt;
        float barWidth = 250;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        // Start is called before the first frame update
        void Start()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            barWidth = rt.sizeDelta.x;
        }

        void Update(){
            Vector2 size = rt.sizeDelta;
            size.x = barWidth * health.GetPercentage()/100;
            rt.sizeDelta = size;
        }
    }
}