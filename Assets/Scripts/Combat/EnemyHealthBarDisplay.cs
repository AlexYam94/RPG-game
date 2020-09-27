using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
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
            rt = GetComponent<RectTransform>();
        }

        // Start is called before the first frame update
        void Start()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
            barWidth = rt.sizeDelta.x;
        }

        // Update is called once per frame
        void Update()
        {
            Health health = fighter.GetTarget();
            if (health != null)
            {
                if (health.IsDead())
                {
                    hideHealthBar();
                }
                else
                {
                    showHealthBar();
                    Vector2 size = rt.sizeDelta;
                    size.x = barWidth * health.GetPercentage() / 100;
                    rt.sizeDelta = size;
                }
            }
        }

        public void showHealthBar()
        {
            GetComponent<Image>().enabled = true;
        }

        public void hideHealthBar()
        {
            GetComponent<Image>().enabled = false;
        }
    }
}