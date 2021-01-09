using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarDisplay : MonoBehaviour
{
    Stamina stamina = null;
    Slider slider = null;
    // Start is called before the first frame update
    void Start()
    {
        stamina = GameObject.FindGameObjectWithTag("Player").GetComponent<Stamina>();
        slider = GetComponent<Slider>();;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = slider.maxValue * stamina.GetFraction();
    }
}
