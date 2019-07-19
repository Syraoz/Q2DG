using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    [HideInInspector]
    public Path path;
    
    [HideInInspector]
    public new EdgeCollider2D collider;

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
                tempPoints[i].y -= path.vOffset;
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

}
