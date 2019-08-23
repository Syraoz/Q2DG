using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TerrainExporter
{
    private Color backgroundColor;
    private Color colliderColor;

    public void ExportToPng()
    {
        Texture2D itemBGTex = Texture2D.blackTexture;
        byte[] itemBGBytes = itemBGTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/KPGT/ExportedImages/terrain.png" , itemBGBytes);
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

}
