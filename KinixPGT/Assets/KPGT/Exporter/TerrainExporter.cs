using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class TerrainExporter
{
    public enum ExportFormat
    {
        PNG, JPG, TGA
    }
    [SerializeField, HideInInspector]
    private Color backgroundColor;
    [SerializeField, HideInInspector]
    private Color colliderColor;
    [SerializeField, HideInInspector]
    private int ppi;
    private string fileName;
    [SerializeField, HideInInspector]
    private int margin;
    [SerializeField, HideInInspector]
    private bool segmentPicture; 

    private Texture2D bgPicture;
    private ExportFormat imgFormat;

    private float textureWidth;
    private float textureHeight;

    public PathGenerator currentPath;

    public TerrainExporter()
    {
        backgroundColor = Color.black;
        colliderColor = Color.white;
        ppi = 100;

    }

    /// TODO:
    ///     LOW PRIORITY    
    ///         Have an option to detect the resolution from the start. Show a warning
    ///     MED PRIORITY
    ///         If the image passes a certain size, give the option to divide the image in a maximum size of 10k x 10k


    public bool ExportTerrainAs()
    { 

        DrawColliderToTexture();
        byte[] itemBGBytes;
        string formatType;
        
        switch (imgFormat)
        {
            case ExportFormat.PNG:
                itemBGBytes = bgPicture.EncodeToPNG();
                formatType = ".png";
                break;
            case ExportFormat.JPG:
                itemBGBytes = bgPicture.EncodeToJPG(100);
                formatType = ".jpg";
                break;
            case ExportFormat.TGA:
                itemBGBytes = bgPicture.EncodeToTGA();
                formatType = ".tga";
                break;
            default:
                itemBGBytes = bgPicture.EncodeToPNG();
                formatType = ".png";
                break;
        }

        //Check if the directory exists
        string checkDir = Application.dataPath + "/Exported_Terrains/";
        if (!Directory.Exists(checkDir))
        {
            Directory.CreateDirectory(Application.dataPath + "/Exported_Terrains/");
        }
        try
        {
            File.WriteAllBytes(Application.dataPath + "/Exported_Terrains/" + fileName + formatType, itemBGBytes);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to save data. Error: " + e.Message);
            return false;
        }
    }

    void DrawColliderToTexture()
    {
        Vector2 maxPos = new Vector2(0, 0);
        Vector2 minPos = new Vector2(0, 0);

        List<Vector2> tempPoints = new List<Vector2>();
        List<Vector2> correctPoints = new List<Vector2>();

        //Check main collider dimensions
        foreach (Vector2 v in currentPath.collider.points)
        {
            if (minPos.x > v.x)
            {
                minPos.x = v.x;
            }
            if (minPos.y > v.y)
            {
                minPos.y = v.y;
            }
            if (maxPos.x < v.x)
            {
                maxPos.x = v.x;
            }
            if (maxPos.y < v.y)
            {
                maxPos.y = v.y;
            }

            tempPoints.Add(v);
        }

        //Check subpaths
        List<List<Vector2>> subTempPoints = new List<List<Vector2>>();
        List<List<Vector2>> subCorrectPoints = new List<List<Vector2>>();

        if(currentPath.subColliders.Count != 0)
        {
            for (int i = 0; i < currentPath.subColliders.Count; i++)
            {
                subCorrectPoints.Add(new List<Vector2>());
                subTempPoints.Add(new List<Vector2>());
                foreach (Vector2 v in currentPath.subColliders[i].points)
                {
                    if (minPos.x > v.x)
                    {
                        minPos.x = v.x;
                    }
                    if (minPos.y > v.y)
                    {
                        minPos.y = v.y;
                    }
                    if (maxPos.x < v.x)
                    {
                        maxPos.x = v.x;
                    }
                    if (maxPos.y < v.y)
                    {
                        maxPos.y = v.y;
                    }
                    subTempPoints[i].Add(v);
                    subCorrectPoints[i].Add(new Vector2());
                }
            }
        }

        //Get the image dimensions, make sure it doesnt exceed 10,000 x 10,000
        textureWidth = Vector2.Distance(new Vector2(minPos.x, 0), new Vector2(maxPos.x, 0));
        textureHeight = Vector2.Distance(new Vector2(0, minPos.y), new Vector2(0, maxPos.y));

        if (textureHeight * ppi > 10000 || textureWidth * ppi > 10000)
        {
            Debug.LogWarning("File dimensions exceed 10000 pixels");
        }
        else
        {
            if (ppi < 1) {ppi = 1;}

            bgPicture = new Texture2D((int)(textureWidth * ppi) + 2 + margin, (int)(textureHeight * ppi) + 2 + margin);

            //Center the main path collider
            correctPoints = CenterCollider(tempPoints, minPos);

            //Center the subpath colliders
            for (int i = 0; i < currentPath.subColliders.Count; i++)
            {
                subCorrectPoints[i] = CenterCollider(subTempPoints[i], minPos);
            }

            SetBackgroundColor();

            //Draw main path lines
            for (int i = 0; i < correctPoints.Count - 1; i++)
            {
                DrawLine(correctPoints[i], correctPoints[i + 1], colliderColor);
            }

            //Draw subpath lines
            for (int i = 0; i < subCorrectPoints.Count; i++)
            {
                for(int j = 0; j < subCorrectPoints[i].Count - 1; j++)
                {
                    DrawLine(subCorrectPoints[i][j], subCorrectPoints[i][j + 1], colliderColor);
                }
            }
        }
    }

    void SetBackgroundColor()
    {
        for(int i = 0; i < bgPicture.width; i++)
        {
            for(int j = 0; j < bgPicture.height; j++)
            {
                bgPicture.SetPixel(i, j, backgroundColor);
            }
        }
    }

    List<Vector2> CenterCollider(List<Vector2> tPs, Vector2 minPos)
    {
        List<Vector2> correctPositions = new List<Vector2>();

        if(minPos.x > 0)
        { //RESTA X
            if (minPos.y > 0)
            { //RESTA Y
                foreach(Vector2 v in tPs)
                {
                    correctPositions.Add(new Vector2(v.x - Mathf.Abs(minPos.x), v.y - Mathf.Abs(minPos.y)));
                }
            }
            else if (minPos.y < 0)
            { //SUMA Y
                foreach (Vector2 v in tPs)
                {
                    correctPositions.Add(new Vector2(v.x - Mathf.Abs(minPos.x), v.y + Mathf.Abs(minPos.y)));
                }
            }
        }
        else if (minPos.x < 0)
        { //SUMA X
            if (minPos.y > 0)
            { //RESTA Y
                foreach (Vector2 v in tPs)
                {
                    correctPositions.Add(new Vector2(v.x + Mathf.Abs(minPos.x), v.y - Mathf.Abs(minPos.y)));
                }
            }
            else if (minPos.y < 0)
            { //SUMA Y
                foreach (Vector2 v in tPs)
                {
                    correctPositions.Add(new Vector2(v.x + Mathf.Abs(minPos.x), v.y + Mathf.Abs(minPos.y)));
                }
            }
        }

        return correctPositions;
    }

    public void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {

        int width = (int)((p2.x * ppi) - (p1.x * ppi) + 1);
        int height = (int)((p2.y * ppi) - (p1.y * ppi) + 1);
        int dx1, dy1, dx2, dy2;

        dx1 = 0;
        dy1 = 0;
        dx2 = 0;
        dy2 = 0;

        if (width < 0) { dx1 = -1; }
        else if (width > 0) { dx1 = 1; }

        if (height < 0) { dy1 = -1; }
        else if (height > 0) { dy1 = 1; }

        if (width < 0) { dx2 = -1; }
        else if (width > 0) { dx2 = 1; }

        int longest = Mathf.Abs(width);
        int shortest = Mathf.Abs(height);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(height);
            shortest = Mathf.Abs(width);

            if (height < 0)
            {
                dy2 = -1;
            } else if (height > 0)
            {
                dy2 = 1;
            }
            dx2 = 0;
        }

        int numerator = longest >> 1;
        int drawingX = (int)(p1.x* ppi +1);
        int drawingY = (int)(p1.y* ppi +1);

        for(int i = 0; i <= longest; i++)
        {
            bgPicture.SetPixel(drawingX + (margin / 2), drawingY + (margin / 2), color);
            numerator += shortest;
            if(!(numerator < longest))
            {
                numerator -= longest;
                drawingX += dx1;
                drawingY += dy1;
            }
            else
            {
                drawingX += dx2;
                drawingY += dy2;
            }
        }
    }

    public void CheckDimensions()
    {

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
    public int Margin
    {
        get
        {
            return margin;
        }
        set
        {
            margin = value;
        }
    }
    public ExportFormat ImageFormat
    {
        get
        {
            return imgFormat;
        }
        set
        {
            imgFormat = value;
        }
    }

}
