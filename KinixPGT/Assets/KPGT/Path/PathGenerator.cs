using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    [HideInInspector]
    public Path path;
    public List<Vector2> playerRoute;

    [HideInInspector]
    public EdgeCollider2D collider;

    private float[,] noiseMap;

    //Noise Tests:
    public int mapW;
    public int mapH;
    public float noiseScale;

    //Floats
    private float offset;
    private float amplitude;
    private float frequency;

    public void GeneratePath()
    {
        path = new Path(transform.position);
    }

    public void UpdateCollider()
    {
        if (collider != null)
        {
            Vector2[] tempPoints = path.GetPoints;
            for (int i = 0; i < path.NumPoints; i++)
            {
                tempPoints[i].y -= offset;
            }

            collider.points = tempPoints;
        }
    }

    public void ToggleCollider()
    {
        if (gameObject.GetComponent<EdgeCollider2D>() == null)
        {
            collider = gameObject.AddComponent<EdgeCollider2D>();
        }
        else
        {
            collider = gameObject.GetComponent<EdgeCollider2D>();
            DestroyImmediate(collider);
        }
    }

    public void RandomizeTerrain()
    {
        //get point x and point x + 1
        //add new points to each segment, the amount depends on frequency
        //run and give true to a bool to let know, we already made a generation to this route / terrain so there aren't more iterative generations
        //assign new possible y values relative to a noise using the amplitude (Perlin, to smooth out the heigh variations)
        if (collider != null)
        {
            float distanceX;
            float distanceY;
            float subDivisionX;

            List<Vector2> tempPoints = new List<Vector2>();

            foreach(Vector2 v in path.GetPoints)
            {
                tempPoints.Add(v); //Convert to list for easy insertion
            }

            for (int i = 0; i < path.NumSegments; i++)
            {
                distanceX = tempPoints[i + 1].x - tempPoints[i].x; //Distance between points, warning of negatives
                distanceY = tempPoints[i + 1].y - tempPoints[i].y; //distance between points in y;

                subDivisionX = Mathf.Abs(distanceX) / frequency + 1; //The distance between each subdivision in X

                for (int s = 0; s < frequency; s++)
                {
                    Vector2 subDiv = new Vector2(tempPoints[i].x + subDivisionX * (s+1), 0);

                    tempPoints.Insert(i, subDiv);
                    //for each subdivision, the subDivisionX is multiplied by * "s", meaning in which subdivision we are.
                }

                //we need to find a way to put subdivisions relative to the frequency
                //Later: a way to noise the positions as a toggable option. User can choose to keep it clean or not
                //alter the height of the points with a perlin noise.
            }
            collider.points = tempPoints.ToArray();
        }
    }

    void NoiseGeneration()
    {
        noiseMap = Noise.GenerateNoiseMap(mapW, mapH, noiseScale);
        //generate a texture2d. Maybe slice one y row from the texture to use as the rng for the amplitude
    }

    public float VOffset
    {
        get
        {
            return offset;
        }
        set
        {
            offset = value;
        }
    }

    public float TAmplitude
    {
        get
        {
            return amplitude;
        }
        set
        {
            amplitude = value;
        }
    }

    public float TFrequency
    {
        get
        {
            return frequency;
        }
        set
        {
            frequency = value;
        }
    }
}
