using UnityEngine;

public class PlantHandler : MonoBehaviour
{

    public GameObject pot;
    public GameObject stemPrefab;
    public GameObject branchPrefab;
    public GameObject leafPrefab;
    public Material leafMaterial;

    public UnityEngine.UI.Slider daysLivedSlider;
    public UnityEngine.UI.Slider waterSlider;
    public UnityEngine.UI.Button regenerateButton;

    private CustomPlant ourPlant;
    
    private void Start()
    {
        // Necessary process for creating a new instance since we are using Mono Behaviour
        ourPlant = new GameObject("Plant").AddComponent<CustomPlant>();
        ourPlant.Initialize(stemPrefab, branchPrefab, leafPrefab, pot);
        Destroy(ourPlant.gameObject);

        UpdateLifeTime(daysLivedSlider.value);
        daysLivedSlider.onValueChanged.AddListener(UpdateLifeTime);

        UpdateWaterStatus(waterSlider.value);
        waterSlider.onValueChanged.AddListener(UpdateWaterStatus);

        regenerateButton.onClick.AddListener(RegeneratePlant);
    }

    void RegeneratePlant()
    {
        ourPlant.delete();

        // Necessary process for creating a new instance since we are using Mono Behaviour
        ourPlant = new GameObject("Plant").AddComponent<CustomPlant>();
        ourPlant.Initialize(stemPrefab, branchPrefab, leafPrefab, pot);
        Destroy(ourPlant.gameObject);

        ourPlant.updatePlant();
        UpdateLifeTime(0);
        UpdateWaterStatus(0);
    }

    void UpdateLifeTime(float val)
    {
        float daysLived = daysLivedSlider.value;
        if (daysLived < waterSlider.value) { waterSlider.value = daysLived; }

        float daysRatio = daysLived / daysLivedSlider.maxValue;
        ourPlant.updateScale(daysRatio);
        ourPlant.updatePlant();
    }

    void UpdateWaterStatus(float val)
    {
        float waterValue = waterSlider.value;
        if(daysLivedSlider.value < waterValue) { daysLivedSlider.value = waterValue; }

        UpdateLeafColor(waterValue);
        float waterRatio = waterValue / waterSlider.maxValue;
        ourPlant.updateRotation(waterRatio);
        ourPlant.updatePlant();
    }

    // Change the leaf's material color depending on the water status
    void UpdateLeafColor(float daysSinceWater)
    {
        float waterSliderMax = waterSlider.maxValue;

        // Define the different colors of a leaf
        Color greenColor = new Color(0, 128 / 255f, 0);
        Color yellowColor = new Color(255 / 255f, 255 / 255f, 0);
        Color brownColor = new Color(139 / 255f, 69 / 255f, 19 / 255f);

        Color resultingColor;
        // Slowly fade the color from green to yellow
        if (daysSinceWater <= waterSliderMax / 2)
        {
            float halfway = waterSliderMax / 2;

            resultingColor =
                (greenColor * (halfway - daysSinceWater) / halfway) +
                (yellowColor * daysSinceWater / halfway);
        }
        // Slowly fade the color from yellow to brown
        else
        {
            resultingColor =
                (yellowColor * (waterSliderMax - daysSinceWater) / (waterSliderMax / 2)) +
                (brownColor * (daysSinceWater - waterSliderMax / 2) / (waterSliderMax / 2));
        }

        // Update the color of the leafs material
        leafMaterial.SetColor("_Color", resultingColor);
    }


}
