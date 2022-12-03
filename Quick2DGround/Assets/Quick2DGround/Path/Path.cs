using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Path
{
    //Propieties
    [SerializeField, HideInInspector]
    List<Vector2> points;

    public Path(Vector2 startPos)
    {
        points = new List<Vector2>
        {
            startPos,
        };
    }

    public Vector2 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }

    public int NumSegments
    {
        get
        {
            return points.Count - 1;
        }
    }

    public Vector2[] GetPoints
    {
        get
        {
            return points.ToArray();
        }
    }

    public List<Vector2> SetPoints
    {
        set
        {
            points = value;
        }
    }

    public void AddPoint(Vector2 pos)
    {
        points.Add(pos);
    }

    public void AddPointStart(Vector2 pos)
    {
        points.Insert(0, pos);
    }

    public void RemovePoint(int pointIndex)
    {
        points.RemoveAt(pointIndex);
    }

    public void MovePoint(int i, Vector2 pos)
    {
        points[i] = pos;
    }
}
