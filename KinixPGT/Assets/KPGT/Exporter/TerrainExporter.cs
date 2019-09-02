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
    private int margin;

    private Texture2D bgPicture;

    private float textureWidth;
    private float textureHeight;

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
        List<Vector2> correctPoints = new List<Vector2>();

        // maxPos.Set(currentPath.path.GetPoints[0].x, currentPath.path.GetPoints[0].y);
        // minPos.Set(currentPath.path.GetPoints[0].x, currentPath.path.GetPoints[0].y);

        if (currentPath != null)
        {

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

            textureWidth = Vector2.Distance(new Vector2(minPos.x, 0), new Vector2(maxPos.x, 0));
            textureHeight = Vector2.Distance(new Vector2(0, minPos.y), new Vector2(0, maxPos.y));
            if (ppi < 1)
            {
                ppi = 1;
            }

            bgPicture = new Texture2D((int)(textureWidth * ppi) + 5 + margin, (int)(textureHeight * ppi) + 5 + margin);

            correctPoints = CenterCollider(tempPoints, minPos);

            SetBackgroundColor();

            for (int i = 0; i < correctPoints.Count - 1; i++)
            {
                DrawLine(correctPoints[i], correctPoints[i + 1], colliderColor);
            }
        }
        else
        {
            Debug.LogError("Terrain not selected");
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
}
