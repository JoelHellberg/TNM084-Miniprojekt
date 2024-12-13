using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SliderValueText : MonoBehaviour
{
    public Slider slider;
    private TMP_Text textComp;

    void Awake()
    {
        //slider = GetComponent<Slider>();
        textComp = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateText(slider.value);
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float val)
    {
        float floatValue = slider.value;
        int intValue = (int)Math.Round(floatValue, 0);
        textComp.text = intValue.ToString();
    }
}
