using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Default class to update the text field to the slider value
public class SliderValueText : MonoBehaviour
{
    public Slider slider;
    private TMP_Text textComp;

    void Awake()
    {
        textComp = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateText(slider.value);

        // Update the text each time the slider value changes
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float val)
    {
        // Update the text value to the slider value
        float floatValue = slider.value;
        int intValue = (int)Math.Round(floatValue, 0);
        textComp.text = intValue.ToString();
    }
}
