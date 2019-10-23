using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    public enum SEType
    {
        none, cliff, wall
    };

    [HideInInspector]
    public Path path;
    List<Vector2> playerRoute;
    List<Vector2> noisedSegments;

    [HideInInspector]
    public EdgeCollider2D collider;

    [SerializeField]
    private float[,] noiseMap;

    //Noise Tests:
    private int mapW = 10;
    private int mapH = 1;

    //Floats
    private float seValue;
    private float amplitude;
    private int frequency;
    private float scale;

    private int currentSeed;

    //Start End Types
    private SEType terrainStart;
    private SEType terrainEnd;


    public void GeneratePath()
    {
        path = new Path(transform.position);
    }   

    public void UpdateCollider()
    {
        if (collider != null)
        {
           RandomizeTerrain();
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

    //We might need a current Y for the algorithm, so even if the noise says the terrain should go higher, there's a few rules 
    //that it must follow, so it goes higher again, but not by much

    //We also need to apply this to the X value, by a much much much smaller value as the points can end up crossing their X values
    //making intersections in the collider

    //A value of amplitude relates to the scale of the noise
    //A value to know what's the angle of the current segment (NORMAL)
    //A value of the angle formed between the previous and next segment (CROSS ANGLES) 

    //We might need a new type of data/object for each segment to store it's information so we can access it later on 

    public void RandomizeTerrain()
    {

        NoiseGeneration();

        if (collider != null)
        {
            //Subdivision
            //Final product is a subdivided list of points of the edge collider: tempPoints

            List<Vector2> tempPoints = new List<Vector2>();
            List<Vector2> refPoints = new List<Vector2>();
            foreach (Vector2 v in path.GetPoints)
            {
                tempPoints.Add(v);
                refPoints.Add(v);
            }

            float magnitud;
            Vector2 direction;

            for (int i = 0; i < path.NumSegments; i++)
            {
                direction = refPoints[i + 1] - refPoints[i];
                magnitud = direction.magnitude;
                direction.Normalize();

                for (int s = 1; s < frequency;  s++)
                {
                    Vector2 newPoint = refPoints[i] + direction * (magnitud / frequency) * (s);
                    tempPoints.Insert(i + ((frequency - 1) * i) + (s), newPoint);
                   
                }
            }

            if(terrainStart == SEType.cliff)
            {
                tempPoints.Insert(0, new Vector2(tempPoints[0].x, tempPoints[0].y - seValue));
            }
            if(terrainStart == SEType.wall)
            {
                tempPoints.Insert(0, new Vector2(tempPoints[0].x, tempPoints[0].y + seValue));
            }
            if(terrainEnd == SEType.cliff)
            {
                tempPoints.Insert(tempPoints.Count, new Vector2(tempPoints[tempPoints.Count-1].x, tempPoints[tempPoints.Count-1].y - seValue));
            }
            if(terrainEnd == SEType.wall)
            {
                tempPoints.Insert(tempPoints.Count, new Vector2(tempPoints[tempPoints.Count-1].x, tempPoints[tempPoints.Count-1].y + seValue));
            }

            if (!(amplitude <= 0))
            {
                Random.InitState(currentSeed);

                float offsetRandomX = Random.value * 1000;
                float offsetRandomY = Random.value * 1000;

                for (int i = 0; i < tempPoints.Count; i++)
                {

                    // Nuevas capas de ruidos, no todo en uno solo

                    float x = Mathf.PerlinNoise(i * (amplitude / 10 ) / tempPoints.Count + offsetRandomX, 0 + offsetRandomY);
                    float y = Mathf.PerlinNoise(i * amplitude / tempPoints.Count + offsetRandomX, 1000 + offsetRandomY);



                    tempPoints[i] += ((new Vector2(x, y) * 2F) - new Vector2(1, 1)) * 2;
                }

            }

            collider.points = tempPoints.ToArray();
        }

    }

    void NoiseGeneration()
    {
        noiseMap = Noise.GenerateNoiseMap(mapW, mapH, frequency, scale);
    }

    public float VOffset
    {
        get
        {
            return seValue;
        }
        set
        {
            seValue = value;
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

    public float NScale
    {
        get
        {
            return scale;
        }
        set
        {
            scale = value;
        }
    }
    
    public SEType EndTerrain
    {
        get
        {
            return terrainEnd;
        }
        set
        {
            terrainEnd = value;
        }
    }

    public SEType StartTerrain
    {
        get
        {
            return terrainStart;
        }
        set
        {
            terrainStart = value;
        }
    }

    public int Seed
    {
        get
        {
            return currentSeed;
        }
        set
        {
            currentSeed = value;
        }
    }
}
