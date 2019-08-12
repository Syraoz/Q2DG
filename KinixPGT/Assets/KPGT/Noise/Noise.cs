using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //Width is how many values will be per segment
    //Height should be 1 only, as we will be getting a single stripe of the noise map
    //Densitiy, how many segments there are
    //Amplitude, how higher or lower can the terrain go
    //Escale, how crazy is the noise

    public static float[,] GenerateNoiseMap(int width, int height, float density, float scale)
    {
        float[,] noiseMap = new float[(int)(width * density), height];

        if(scale <= 0)
        {
            scale = 0.0001f;
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
