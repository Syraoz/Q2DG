using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{

    [HideInInspector]
    public Path path;

    public void GeneratePath()
    {
        path = new Path(transform.position);
    }

}
