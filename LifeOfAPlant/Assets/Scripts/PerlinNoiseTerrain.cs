using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class PerlinNoiseTerrain : MonoBehaviour
{

    //Get value from slider for days without water
    public UnityEngine.UI.Slider waterSlider;

    public float daysWithoutWater;

    public float depth = 20;

    public float scaleFactor = 0.2f; // Lägre värden minskar höjden

    public Material material;

    public float metallicValue = 0.1f;


    public int width = 50;
    public int height = 50;

    private void Start()
    {


        Debug.Log("Hej");

        //Log value from the slider (Test-phase)
        if (waterSlider != null)
        {
            daysWithoutWater = waterSlider.value;
            Debug.Log("Initial Watercounter: " + daysWithoutWater);
        }
        else
        {
            Debug.LogError("WaterSlider is not assigned in the Inspector!");
        }

        Debug.Log("Metallic Value: " + metallicValue);
    }

    //Update real-time
    void Update()
    {

        Terrain terrain = GetComponent<Terrain>();


        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        // Uppdatera Watercounter baserat på sliderns värde
        if (waterSlider != null)
        {
            daysWithoutWater = waterSlider.value;
           // Debug.Log("Current Watercounter: " + daysWithoutWater);
        }


        //0->Light
        //1->Dark
        if (daysWithoutWater <= 0)
        {
            material.SetFloat("_Metallic", metallicValue);
        }
        else
        {
            float adjustedValue = metallicValue / (daysWithoutWater/30); // Mjukare skalning
            adjustedValue = Mathf.Clamp(adjustedValue, 0.05f, 2f); // Begränsar värdet mellan 0.1 och 1
            material.SetFloat("_Metallic", adjustedValue);
        }

        Debug.Log("Metallic Value: " + metallicValue);

    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {

        terrainData.heightmapResolution = width + 1 ;

        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData; 

    }

    //2D- float array
    float[,] GenerateHeights()
    {
        //Size of array, grid of floats
        float[,] heights = new float[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y =  0; y < height; y++)
            {
                //Perlin noise value
                heights[x, y] = CalculateHeight(x,y);
            }
        }

        return heights;

    }



    float CalculateHeight(int x, int y)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = daysWithoutWater/50f;
        int octaves = 4; // Antal lager av Perlin Noise
        float persistence = 0.5f; // Hur mycket varje lager bidrar

        for (int i = 0; i < octaves; i++)
        {
            float xCoord = (float)x / width * frequency;
            float yCoord = (float)y / height * frequency;

            // Lägg till varje lager av Perlin Noise
            total += CalculatePerlinNoise(xCoord, yCoord) * amplitude;

            amplitude *= persistence; // Minskande bidrag för varje lager
            frequency *= 2;           // Dubblar frekvensen för varje lager
        }

        // Normalisera värdet och justera skalan
        return Mathf.Clamp(total, 0f, 1f) * scaleFactor;
    }



    float CalculatePerlinNoise(float x, float y)
    {
        int[] permutation = {
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140,
        36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234,
        75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237,
        149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
        27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105,
        92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73,
        209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
        164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
        147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
        28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
        101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224,
        232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144,
        12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214,
        31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
        138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215,
        61, 156, 180
         };

        int[] p;
        p = new int[512];

        for (int i = 0; i < 256; i++)
        {
            p[i] = permutation[i];
            p[i + 256] = permutation[i];
        }

        // Hitta koordinater
        int xi = Mathf.FloorToInt(x) & 255;
        int yi = Mathf.FloorToInt(y) & 255;

        // Hitta relativa positioner i rutan
        float xf = x - Mathf.Floor(x);
        float yf = y - Mathf.Floor(y);

        // Beräkna fade-kurvor för interpolering
        float u = Fade(xf);
        float v = Fade(yf);

        // Hitta hörnpunkter
        int aa = p[p[xi] + yi];
        int ab = p[p[xi] + yi + 1];
        int ba = p[p[xi + 1] + yi];
        int bb = p[p[xi + 1] + yi + 1];

        // Interpolera resultaten från varje hörn
        float x1 = Mathf.Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        float x2 = Mathf.Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
        return Mathf.Clamp(Mathf.Lerp(x1, x2, v), 0f, 1f);


    }



    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 3;
        float u = h < 2 ? x : y;
        float v = h < 2 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }


}
