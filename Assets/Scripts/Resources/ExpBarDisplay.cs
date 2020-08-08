using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using RPG.Stats;
using UnityEngine;

public class ExpBarDisplay : MonoBehaviour
{
    RectTransform rt;
    float barWidth = 250;
    Experience experience = null;
    float currentExp = 0;
    float expToLevelUp = 0;

    private void Awake()
    {
        experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        rt = GetComponent<RectTransform>();
        barWidth = rt.sizeDelta.x;
        expToLevelUp = GetBarMaxNum();
    }

    // private void Start() {
    //     experience.onExperienceGained += UpdateExpBar;
    // }

    public void UpdateExpBar(float experiencePoint)
    {
        
        Vector2 size = rt.sizeDelta;
        currentExp += experiencePoint;
        size.x = barWidth + (currentExp/expToLevelUp * 100);
        if(size.x >= 100){
            currentExp = 0;
            size.x %= 100;
            expToLevelUp = GetBarMaxNum();
        }
        // print("expToLevelUp: " + expToLevelUp);
        // print("currentExp: " + currentExp);
        rt.sizeDelta = size;
    }

    private float GetBarMaxNum()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().GetExpToLevelUp() - experience.GetCurrentExpPoint();
    }
}
