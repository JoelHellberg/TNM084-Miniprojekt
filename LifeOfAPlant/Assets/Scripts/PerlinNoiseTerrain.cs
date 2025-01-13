using UnityEngine;


public class PerlinNoiseTerrain : MonoBehaviour
{

    //H�mta slidern
    public UnityEngine.UI.Slider waterSlider;

    public float daysWithoutWater;

    public float depth = 20; 

    public float scaleFactor = 0.2f;

    public Material material;

    public float metallicValue = 0.1f;


    public int width = 50;
    public int height = 50;

    private void Start()
    {

        //F�rhindra f�r error
        if (waterSlider != null)
        {
            //H�mta v�rde fr�n slidern f�r days without water
            daysWithoutWater = waterSlider.value;
        }
        else
        {
            Debug.LogError("WaterSlider is not assigned in the Inspector!");
        }

        // Alter the terrain when the value on the waterSlider changes
        UpdateTerrain(waterSlider.value);
        waterSlider.onValueChanged.AddListener(UpdateTerrain);

        // Debug.Log("Metallic Value: " + metallicValue);
    }

    
    //Uppdatera terr�ngen med v�rdet fr�n slidern
    //�ndrar terr�ngen och ytans material
    void UpdateTerrain(float val)
    {
        //Skapa terr�ngen
        Terrain terrain = GetComponent<Terrain>();


        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        // Uppdatera Watercounter baserat p� sliderns v�rde
        if (waterSlider != null)
        {
            daysWithoutWater = waterSlider.value;
            // Debug.Log("Current Watercounter: " + daysWithoutWater);
        }


        //0->Light
        //1->Dark

        //�ndra ytans f�rg f�r att visualisera torr och bl�t jord
        if (daysWithoutWater <= 0)
        {
            material.SetFloat("_Metallic", metallicValue);
        }
        else
        {
            float adjustedValue = metallicValue / (daysWithoutWater / 30); //Skala v�rdet beroende p� sliderns v�rde
            adjustedValue = Mathf.Clamp(adjustedValue, 0.05f, 2f); // Begr�nsar v�rdet mellan 0.o5 och 2
            material.SetFloat("_Metallic", adjustedValue);
        }

        //Debug.Log("Metallic Value: " + metallicValue);
    }


    //Generera terr�ngen
    TerrainData GenerateTerrain(TerrainData terrainData)
    {

        terrainData.heightmapResolution = width + 1 ;

        //Terr�ngens storlek
        terrainData.size = new Vector3(width, depth, height);

        //Terr�ngens h�jdkarta
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData; 

    }

    //Skapar en 2-dimensionell array, ger h�jden som anv�nds i ter�ngen h�jdkarta
    float[,] GenerateHeights()
    {
        //Arrayems storlek, definieras i unity (32x32 f�r tillf�llet)
        float[,] heights = new float[width, height];


        //G�r igenom alla punkter i terr�ngen
        for(int x = 0; x < width; x++)
        {
            for(int y =  0; y < height; y++)
            {
                //Ger h�jden f�r en punkt i terr�ngen, ber�knas av Calculateheight()
                heights[x, y] = CalculateHeight(x,y);
            }
        }

        return heights;

    }



    float CalculateHeight(int x, int y)
    {
        //V�rde �r finjusterade f�r det specifikt �nskade resulatet 
        float total = 0;
        float amplitude = 1; //Hur mycket varje lagar bidrar
        float lacunarity = 2;
        float frequency = daysWithoutWater/50f;
        int octaves = 4; // Antal lager av Perlin Noise
        float persistence = 0.5f; // Hur mycket varje lager bidrar, s�nker p�verkan f�r varje lager


        for (int i = 0; i < octaves; i++)
        {

            //Normalisera
            float xCoord = (float)x / width * frequency;
            float yCoord = (float)y / height * frequency;

            // L�gg till varje lager av Perlin Noise, amplituden g�r att vajre lager p�verkar olika
            total += CalculatePerlinNoise(xCoord, yCoord) * amplitude;

            amplitude *= persistence; // Minskande bidrag f�r varje lager
            frequency *= lacunarity; // Dubblar frekvensen f�r varje lager
        }

        // Normalisera v�rdet (Blev knas om jag inte anv�nder Clamp)
        return Mathf.Clamp(total, 0f, 1f) * scaleFactor;
    }


    //Ber�kna Perlin Noise fr�n vajre punkt (Inspo fr�n labbarna)
    float CalculatePerlinNoise(float x, float y)
    {
        //"Random" pattern
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

        // Ber�kna fade-kurvor f�r interpolering
        float u = xf;
        float v = yf;

        // Hitta h�rnpunkter
        int aa = p[p[xi] + yi];
        int ab = p[p[xi] + yi + 1];
        int ba = p[p[xi + 1] + yi];
        int bb = p[p[xi + 1] + yi + 1];

        // Interpolera resultaten fr�n varje h�rn f�r att f� en mjuk terr�ng
        float x1 = Mathf.Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        float x2 = Mathf.Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
        return Mathf.Clamp(Mathf.Lerp(x1, x2, v), 0f, 1f);


    }



    //B�ttre �verg�ng
    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    //Ber�kna gradienterna, skapar mjukare �verg�ngar
    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 3;
        float u = h < 2 ? x : y;
        float v = h < 2 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }


}
