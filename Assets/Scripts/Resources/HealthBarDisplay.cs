using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Resources
{
    public class HealthBarDisplay : MonoBehaviour
    {

        Health health;
        RectTransform rt;
        float barWidth = 250;

        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            rt = GetComponent<RectTransform>();
            barWidth = rt.sizeDelta.x;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void UpdateHealthBar()
        {
            Vector2 size = rt.sizeDelta;
            size.x = barWidth * health.GetPercentage()/100;
            rt.sizeDelta = size;
        }
    }
}