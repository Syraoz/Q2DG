using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

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
    private float offset;
    private int amplitude;
    private int frequency;
    private float scale;


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

    //WE might need a current Y for the algorithm, so even if the noise says the terrain should go higher, there's a few rules 
    //that it must follow, so it goes higher again, but not by much

    //A value of amplitude that tells the segment how much is it affected by the noise (FLOAT)
    //A value of the current Y relative to the min and max Y to determine how much higher or lower it can go (FLOAT)
    //A value to know what's the angle of the current segment (NORMAL)
    //A value of the angle formed between the previous and next segment (CROSS ANGLES) 
    //A value to keep track of the current Y (FLOAT)

    //We might need a new type of data/object for each segment to store it's information so we can access it later on 

    public void RandomizeTerrain()
    {
        NoiseGeneration();

        if (collider != null)
        {
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

                #region "Old Code"
                //distance.x = Mathf.Abs(refPoints[i + 1].x - refPoints[i].x);
                //distance.y = Mathf.Abs(refPoints[i + 1].y - refPoints[i].y); 
                //CORRECTLY

                //subDivisionX = distance.x / (frequency + 1); 
                //subDivisionY = distance.y / (frequency + 1);
                #endregion

                for (int s = 1; s < frequency + 1; s++)
                {
                    Vector2 newPoint = refPoints[i] + direction * (magnitud / frequency - 1) * (s);

                    int useNoiseAt = Random.Range(0, mapW * frequency);


                    newPoint.y += noiseMap[useNoiseAt, 0] > 0.5 ? (noiseMap[useNoiseAt,0] * amplitude) : (-noiseMap[useNoiseAt, 0] * amplitude);
                    newPoint.x += noiseMap[useNoiseAt, 0] > 0.5 ? (noiseMap[useNoiseAt, 0] * 0.3f) : (-noiseMap[useNoiseAt, 0] * 0.3f);

                    tempPoints.Insert(i + (frequency * i) + (s), newPoint);
                    
                    #region "Old Code"
                    //Vector2 subDiv = new Vector2(refPoints[i].x + subDivisionX * (s), refPoints[i].y + subDivisionY * (s));
                    //WACK
                    //Only works on uphills.

                    //tempPoints.Insert(i + ((frequency * i) + s), subDiv);

                    //CORRECTLY
                    #endregion
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
}
