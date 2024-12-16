using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PerlinNoiseTerrain : MonoBehaviour
{

    //Get the value from the slider
    public float scale = 20;

    public float depth = 20;

    public int width = 50;
    public int height = 50;

    private void Start()
    {
       Terrain terrain = GetComponent<Terrain>();

        terrain.terrainData = GenerateTerrain(terrain.terrainData);
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
        float xCoord = (float) x / width * scale;
        float yCoord = (float) y / height * scale;

        //Do our own perlinnoise later
        return Mathf.PerlinNoise(xCoord, yCoord);
    }





}
