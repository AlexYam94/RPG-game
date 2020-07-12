using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthBarDisplay : MonoBehaviour
    {
        [SerializeField]
        Image background = null;

        Fighter fighter;
        RectTransform rt;
        float barWidth = 250;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
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
            Health health = fighter.GetTarget();
            if(health != null){
                showHealthBar();
                print(health.gameObject.name);
                Vector2 size = rt.sizeDelta;
                size.x = barWidth * health.GetPercentage()/100;
                rt.sizeDelta = size;
            }{
                hideHealthBar();
            }
        }

        public void showHealthBar(){
            GetComponent<Image>().enabled = true;
            background.enabled = true;
        }

        public void hideHealthBar(){
            GetComponent<Image>().enabled = false;
            background.enabled = false;
        }
    }
}