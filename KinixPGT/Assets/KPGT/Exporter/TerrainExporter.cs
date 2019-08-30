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

    private Texture2D bgPicture;

    private int textureWidth;
    private int textureHeight;

    public PathGenerator currentPath;

    public void ExportToPng()
    {
        DrawColliderToTexture();

        //function to save and check if the file was saved
        byte[] itemBGBytes = bgPicture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/KPGT/ExportedImages/" + fileName + ".png" , itemBGBytes);
    }

    void DrawColliderToTexture()
    {
        Vector2 maxPos = new Vector2(0,0);
        Vector2 minPos = new Vector2(0,0);

        List<Vector2> tempPoints = new List<Vector2>();

       // maxPos.Set(currentPath.path.GetPoints[0].x, currentPath.path.GetPoints[0].y);
       // minPos.Set(currentPath.path.GetPoints[0].x, currentPath.path.GetPoints[0].y);

        foreach (Vector2 v in currentPath.path.GetPoints)
        {
            if(minPos.x > v.x)
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

        textureWidth = (int)Vector2.Distance(new Vector2(minPos.x, 0), new Vector2(maxPos.x, 0)) + 1;
        textureHeight = (int)Vector2.Distance(new Vector2(0, minPos.y), new Vector2(0, maxPos.y)) + 1;
        if(ppi < 1)
        {
            ppi = 1;
        }

        bgPicture = new Texture2D(textureWidth * ppi, textureHeight * ppi);

        for(int i = 0; i < tempPoints.Count - 1; i++)
        {
            DrawLine(tempPoints[i], tempPoints[i + 1], colliderColor);
        }
    }

    void SetBackgroundColor()
    {

    }

    public void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        int width = (int)((p2.x - p1.x)/*ppi*/);
        int height = (int)((p2.y - p1.y)/*ppi*/);
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

        int maxWidth = Mathf.Abs(width);
        int maxHeight = Mathf.Abs(height);
        if (!(maxWidth > maxHeight))
        {
            maxWidth = Mathf.Abs(height);
            maxHeight = Mathf.Abs(width);

            if (height < 0)
            {
                dy2 = -1;
            } else if (height > 0)
            {
                dy2 = 1;
            }
            dx2 = 0;
        }

        int numerator = maxWidth >> 1;
        int drawingX = (int)p1.x;
        int drawingY = (int)p1.y;
        for(int i = 0; i <= maxWidth; i++)
        {
            bgPicture.SetPixel(drawingX, drawingY, color);
            numerator += maxHeight;
            if(!(numerator < maxWidth))
            {
                numerator -= maxWidth;
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
