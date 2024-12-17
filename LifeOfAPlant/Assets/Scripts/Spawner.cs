using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using static UnityEditor.Progress;
using UnityEditor.Experimental.GraphView;

public class Spawner : MonoBehaviour
{

    public GameObject pot;
    public GameObject stemPrefab;
    public GameObject branchPrefab;
    public GameObject leafPrefab;

    public UnityEngine.UI.Slider daysLivedSlider;
    public UnityEngine.UI.Slider waterSlider;

    private CustomPlant ourPlant;
    
    private void Start()
    {
        ourPlant = new CustomPlant(stemPrefab, branchPrefab, leafPrefab, pot);

        UpdateLifeTime(daysLivedSlider.value);
        daysLivedSlider.onValueChanged.AddListener(UpdateLifeTime);

        UpdateWaterStatus(waterSlider.value);
        waterSlider.onValueChanged.AddListener(UpdateWaterStatus);
    }

    void UpdateLifeTime(float val)
    {
        float floatValue = daysLivedSlider.value;
        ourPlant.updateScale(floatValue);
        ourPlant.updatePlant();
    }

    void UpdateWaterStatus(float val)
    {
        float floatValue = waterSlider.value;
        ourPlant.updateRotation(floatValue);
        ourPlant.updatePlant();
    }

}
