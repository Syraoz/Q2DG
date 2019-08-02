using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    [HideInInspector]
    public Path path;
    public List<Vector2> playerRoute;
    public List<Vector2> noisedSegments;

    [HideInInspector]
    public EdgeCollider2D collider;

    private float[,] noiseMap;

    //Noise Tests:
    public int mapW;
    public int mapH;
    public float noiseScale;

    //Floats
    private float offset;
    private int amplitude;
    private int frequency;


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

           // collider.points = tempPoints;
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
        NoiseGeneration();

        if (collider != null)
        {
            float distanceX;
            float distanceY;
            float subDivisionX;
            float subDivisionY;

            List<Vector2> tempPoints = new List<Vector2>();
            List<Vector2> refPoints = new List<Vector2>(); 

            foreach (Vector2 v in path.GetPoints)
            {
                tempPoints.Add(v);
                refPoints.Add(v);
            }


            for (int i = 0; i < path.NumSegments; i++)
                {
                distanceX = Mathf.Abs(refPoints[i + 1].x - refPoints[i].x);
                distanceY = Mathf.Abs(refPoints[i + 1].y - refPoints[i].y); 
                //CORRECTLY

                subDivisionX = distanceX / (frequency + 1); 
                subDivisionY = distanceY / (frequency + 1);

                for (int s = 1; s < frequency + 1; s++)
                {

                    Vector2 subDiv = new Vector2(refPoints[i].x + subDivisionX * (s), refPoints[i].y + subDivisionY * (s));
                    //WACK
                    //Only works on uphills.

                    tempPoints.Insert(i + ((frequency * i) + s), subDiv);
                    //CORRECTLY

                    //i = current main punt
                    //Frequency * i = number of points we are skipping 
                    //s = current sub-segment

                }

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

    public int TAmplitude
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

    public int TFrequency
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
