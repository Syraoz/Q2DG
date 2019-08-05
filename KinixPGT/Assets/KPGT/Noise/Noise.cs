using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float density)
    {
        float[,] noiseMap = new float[(int)(width * density), height];

        if(density <= 0)
        {
            density = 0.0001f;
        }

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width * density; x++)
            {
                float sampleX = x / density;
                float sampleY = y / density;

                float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinNoise;

            }
        }
        return noiseMap;
    }
}
