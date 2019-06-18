using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBuilder
{

    public void GenerateBase(Vector2 start, Vector2 end)
    {
        GameObject platform = new GameObject();
        platform.AddComponent<EdgeCollider2D>();
        platform.name = "New Terrain";

        platform.transform.position = start;
        List<Vector2> tempArray = new List<Vector2>();
        float yValue = Random.Range(-1.0f, 1.0f);

        for (int x = (int)start.x; x < (int)end.x; x++)
        {
            tempArray.Add(new Vector2(x, yValue));
            yValue += Random.Range(-1.0f, 1.0f);
        }
        platform.GetComponent<EdgeCollider2D>().points = tempArray.ToArray();
    }
}
