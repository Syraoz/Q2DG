using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TerrainExporter
{
    private Color backgroundColor;
    private Color colliderColor;
    private int ppi;
    private string fileName;

    public PathGenerator currentPath;

    public void ExportToPng()
    {
        //function to pass the collider to the texture
        Texture2D itemBGTex = Texture2D.blackTexture;


        //function to save and check if the file was saved
        byte[] itemBGBytes = itemBGTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/KPGT/ExportedImages/" + fileName + ".png" , itemBGBytes);
    }

    void ColliderToArray()
    {
        //Vector2 

        for (int i = 0; i > currentPath.path.NumPoints; i++)
        {

        }
    }

    public Color BGColor
    {
        get
        {
            return backgroundColor;
        }
        set
        {
            backgroundColor = value;
        }
    }
    public Color LineColor
    {
        get
        {
            return colliderColor;
        }
        set
        {
            colliderColor = value;
        }
    }
    public int PixelsPerUnit
    {
        get
        {
            return ppi;
        }
        set
        {
            ppi = value;
        }
    }
    public string Name
    {
        get
        {
            return fileName;
        }
        set
        {
            fileName = value;
        }
    }
}
