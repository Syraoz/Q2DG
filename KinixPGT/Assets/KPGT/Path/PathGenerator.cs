using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
[System.Serializable]
public class PathGenerator : MonoBehaviour
{

    public enum SEType
    {
        none, cliff, wall
    };

    [HideInInspector]
    public Path path;
    public List<Path> subPaths;

    [SerializeField, HideInInspector]
    public EdgeCollider2D collider;
    public List<EdgeCollider2D> subColliders;
    public List<Vector2> subPathedPoints;

    //Floats, values for the terrain as a whole
    [SerializeField, HideInInspector]
    private float seValue;
    [SerializeField, HideInInspector]
    private float frequency; 
    [SerializeField, HideInInspector]
    private int details; 
    [SerializeField, HideInInspector]
    private float scale;

    [SerializeField, HideInInspector]
    private int currentSeed;
    [SerializeField, HideInInspector]
    private float handlesize;

    //Start End Types
    [SerializeField, HideInInspector]
    private SEType terrainStart;
    [SerializeField, HideInInspector]
    private SEType terrainEnd;

    public PathGenerator()
    {

        frequency = 3;
        details = 3;
        scale = 1;

        handlesize = 1;

        subPaths = new List<Path>();
        subColliders = new List<EdgeCollider2D>();
        subPathedPoints = new List<Vector2>();
    }

    public void GeneratePath()
    {
        path = new Path(transform.position);
    }
    public void ResetSubPaths()
    {
        subColliders = new List<EdgeCollider2D>();
        subPathedPoints = new List<Vector2>();
        subPaths = new List<Path>();
    }

    public void GenerateSubPath(Vector2 startPos, Vector2 connectedPos)
    {
        subPaths.Add(new Path(startPos));
        subPaths[subPaths.Count - 1].AddPoint(connectedPos);
        subPathedPoints.Add(subPaths[subPaths.Count - 1][0]);
        subColliders.Add(gameObject.AddComponent<EdgeCollider2D>());
    }

    public void UpdateCollider()
    {
        if (collider != null)
        {
            RandomizeTerrain();
            if(subColliders.Count != 0)
            {
                RandomizeSubTerrains();
            }
        }
    }

    //We might need a current Y for the algorithm, so even if the noise says the terrain should go higher, there's a few rules 
    //that it must follow, so it goes higher again, but not by much

    //We also need to apply this to the X value, by a much much much smaller value as the points can end up crossing their X values
    //making intersections in the collider

    //A value of amplitude relates to the scale of the noise
    //A value to know what's the angle of the current segment (NORMAL)
    //A value of the angle formed between the previous and next segment (CROSS ANGLES) 


    /// <summary>
    /// TODO: 
    ///        HIGH PRIORITY
    ///                 Upgrade the method of terrain generation
    ///                 Take in acount the size of the segment for sub segmentation
    ///                 Take in account angle for height variations
    ///                 More values to customize
    ///        MED PRIORITY
    ///                 Being able to start the collider from the second point, leaving a gap
    /// </summary>
    public void RandomizeTerrain()
    {

        if (collider != null)
        {

            //Subdivision, final product is a subdivided list of points of the edge collider: tempPoints
            //refpoints are used to keep track of the original points

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

                for (int s = 1; s < details; s++)
                {
                    Vector2 newPoint = refPoints[i] + direction * (magnitud / details) * (s);
                    tempPoints.Insert(i + ((details - 1) * i) + (s), newPoint);

                }
            }

            //Set the ending and start of the terrain, this only should affect the main path

            if (terrainStart == SEType.cliff)
            {
                tempPoints.Insert(0, new Vector2(tempPoints[0].x, tempPoints[0].y - seValue));
            }
            if (terrainStart == SEType.wall)
            {
                tempPoints.Insert(0, new Vector2(tempPoints[0].x, tempPoints[0].y + seValue));
            }
            if (terrainEnd == SEType.cliff)
            {
                tempPoints.Insert(tempPoints.Count, new Vector2(tempPoints[tempPoints.Count - 1].x, tempPoints[tempPoints.Count - 1].y - seValue));
            }
            if (terrainEnd == SEType.wall)
            {
                tempPoints.Insert(tempPoints.Count, new Vector2(tempPoints[tempPoints.Count - 1].x, tempPoints[tempPoints.Count - 1].y + seValue));
            }

            //Here starts the procedural generation of the terrain

            if (!(frequency <= 0))
            {
                Random.InitState(currentSeed);

                float offsetRandomX = Random.value * 1000;
                float offsetRandomY = Random.value * 1000;

                for (int i = 0; i < tempPoints.Count; i++)
                {

                    // Nuevas capas de ruidos, no todo en uno solo
                    //agregar la escala del ruido aqui

                    float x = Mathf.PerlinNoise(i * (frequency / 10) / tempPoints.Count + offsetRandomX, 0 + offsetRandomY);
                    float y = Mathf.PerlinNoise(i * frequency / tempPoints.Count + offsetRandomX, 1000 + offsetRandomY);



                    tempPoints[i] += ((new Vector2(x, y*scale) * 2F) - new Vector2(1, 1)) * 2;
                }

            }

            collider.points = tempPoints.ToArray();
        }

    }

    public void RandomizeSubTerrains()
    {
            
        for (int p = 0; p < subPaths.Count; p++)
        {
            if (subColliders[p] != null)
            {
                List<Vector2> tempPoints = new List<Vector2>();
                List<Vector2> refPoints = new List<Vector2>();
                foreach (Vector2 v in subPaths[p].GetPoints)
                {
                    tempPoints.Add(v);
                    refPoints.Add(v);
                }

                float magnitud;
                Vector2 direction;

                for (int i = 0; i < subPaths[p].NumSegments; i++)
                {
                    direction = refPoints[i + 1] - refPoints[i];
                    magnitud = direction.magnitude;
                    direction.Normalize();

                    for (int s = 1; s < details; s++)
                    {
                        Vector2 newPoint = refPoints[i] + direction * (magnitud / details) * (s);
                        tempPoints.Insert(i + ((details - 1) * i) + (s), newPoint);

                    }
                }

                if (!(frequency <= 0))
                {
                    Random.InitState(currentSeed);

                    float offsetRandomX = Random.value * 1000;
                    float offsetRandomY = Random.value * 1000;

                    for (int i = 0; i < tempPoints.Count; i++)
                    {

                        // Nuevas capas de ruidos, no todo en uno solo
                        //agregar la escala del ruido aqui

                        float x = Mathf.PerlinNoise(i * (frequency / 10) / tempPoints.Count + offsetRandomX, 0 + offsetRandomY);
                        float y = Mathf.PerlinNoise(i * frequency / tempPoints.Count + offsetRandomX, 1000 + offsetRandomY);



                        tempPoints[i] += ((new Vector2(x, y * scale) * 2F) - new Vector2(1, 1)) * 2;
                    }

                }
                
                
                //Get the main collider
                List<Vector2> colliderPoints = new List<Vector2>();
                foreach (Vector2 v in collider.points)
                {
                    colliderPoints.Add(v);
                }

                //check if the point is branched from the mainpath
                for (int i = 0; i < path.NumPoints; i++)
                {
                    if (subPaths[p][0] == path[i])
                    {
                        tempPoints[0] = colliderPoints[i * details + 1];
                    }
                }

                for (int i = 0; i < subPaths.Count; i++)
                {
                    if (i != p)
                    {
                        //Get current subpath
                        List<Vector2> subColliderPoints = new List<Vector2>();
                        foreach (Vector2 v in subColliders[i].points)
                        {
                            subColliderPoints.Add(v);
                        }

                        for (int j = 1; j < subPaths[i].NumPoints; j++)
                        {
                            if (subPaths[p][0] == subPaths[i][j])
                            {
                                tempPoints[0] = subColliderPoints[j * details];
                            }
                        }
                    }
                }
                
                subColliders[p].points = tempPoints.ToArray();
            }
        }
    }

    /*public void UpdatePath()
    {
        List<Vector2> tempPoints = new List<Vector2>();
        foreach (Vector2 v in path.GetPoints)
        {
            tempPoints.Add(v);
        }
        for (int i = 0; i < tempPoints.Count; i++)
        {
            tempPoints[i] += (Vector2)gameObject.transform.position;
        }
        path.SetPoints = tempPoints;
    }*/

    public void GenerateNewSeed()
    {
        currentSeed = Random.Range(100000000, 999999999);
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
    public int TDetails
    {
        get
        {
            return details;
        }
        set
        {
            details = value;
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
    public float DoHandleSize
    {
        get
        {
            return handlesize;
        }
        set
        {
            handlesize = value;
        }
    }

    public int NumSubPoints
    {
        get
        {
            return subPathedPoints.Count;
        }
    }
    public Vector2 GetSubPoint(int j)
    {
        return subPathedPoints[j];
    }
    public int GetSubPointAt(Vector2 pos)
    {
        for(int i = 0; i < NumSubPoints; i++)
        {
            if(subPathedPoints[i] == pos)
            {
                return i;
            }
        }
        return -1;
    }
    public void RemoveSubPoint(int i)
    {
        subPathedPoints.RemoveAt(i);
    }
    public void MoveSubPoint(int y, Vector2 pos)
    {
        for (int i = 0; i < subPaths.Count; i++)
        {
            if (subPathedPoints[y] == subPaths[i][0])
            {
                subPaths[i].MovePoint(0, pos);
                subPathedPoints[i] = pos;
            }
        }
    }
    public int GetSubPaths
    {
        get
        {
            return subPaths.Count;
        }
    }

}
