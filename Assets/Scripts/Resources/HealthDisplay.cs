using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        Text text;

        Health health;

        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            text = GetComponent<Text>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            text.text = String.Format("{0:0}", health.GetPercentage()) + "%";
        }
    }
}