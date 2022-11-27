using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //Width is how many values will be per segment (INT)
    //Densitiy, how many segments is the X multiplied by (INT)
    //Escale, How smooth the noise it, bigger scale = smoother, smaller scale = more erractic (FLOAT)
    //HEIGHT, Constant (INT 1)

    public static float[,] GenerateNoiseMap(int width, int height, float density, float scale)
    {
        //IMPLEMENTAR UN SISTEMA DE SEED

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
