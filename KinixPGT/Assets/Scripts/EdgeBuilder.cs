using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBuilder : MonoBehaviour
{
    public Vector2 startPos;
    public Vector2 endPos;

    public void GenerateBase()
    {
        GameObject platform = new GameObject();
        platform.AddComponent<EdgeCollider2D>();

        platform.transform.position = startPos;
        List<Vector2> tempArray = new List<Vector2>();
        float yValue = Random.Range(-1.0f, 1.0f);

        for (int x = (int)startPos.x; x < (int)endPos.x; x++)
        {
            tempArray.Add(new Vector2(x, yValue));
            yValue += Random.Range(-1.0f, 1.0f);
        }
        platform.GetComponent<EdgeCollider2D>().points = tempArray.ToArray();
    }
}
