using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        Text text;

        BaseStats baseStat;

        private void Awake()
        {
            baseStat = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<Text>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // print("current level: " + baseStat.GetLevel());
            text.text = ""+baseStat.GetLevel();
        }
    }
}