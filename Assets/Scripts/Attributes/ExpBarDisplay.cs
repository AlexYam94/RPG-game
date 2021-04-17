using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarDisplay : MonoBehaviour
{
    RectTransform rt;
    float barWidth = 250;
    Experience experience = null;
    float currentExp = 0;
    float expToLevelUp = 0;
    Slider slider = null;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        rt = GetComponent<RectTransform>();
        barWidth = rt.sizeDelta.x;
    }

    private void Start() {
        expToLevelUp = GetBarMaxNum(); 
    }

    public void UpdateExpBar(float experiencePoint)
    {
        
        // Vector2 size = rt.sizeDelta;
        currentExp += experiencePoint;
        // size.x = barWidth + (currentExp/expToLevelUp * 100);
        // if(size.x >= 100){
        //     currentExp = 0;
        //     size.x %= 100;
        //     expToLevelUp = GetBarMaxNum();
        // }
        // rt.sizeDelta = size;
        print(currentExp/expToLevelUp);
        slider.value = slider.value + (currentExp/expToLevelUp);
        if(slider.value >= 1){
            currentExp = 0;
            slider.value = 0;
            expToLevelUp = GetBarMaxNum();
        }
    }

    private float GetBarMaxNum()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().GetExpToLevelUp() - experience.GetCurrentExpPoint();
    }
}
