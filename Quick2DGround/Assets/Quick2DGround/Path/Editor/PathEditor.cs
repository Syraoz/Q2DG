using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGenerator))]
public class PathEditor : Editor
{
    PathGenerator creator;
    Path path;
    List<Path> creatorSubPaths;

    /// <summary>
    /// TODO:
    ///     MED PRIORITY
    ///             Implement being able to delete points where a subpath starts, making it connect to the closest point possible.
    ///             If there are no more remaining points, it becomes attatched to the previous subpath or main path
    ///     LOW PRIORITY
    ///             Be able to Undo/Redo actions when deleting a subpath
    /// </summary>

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            fixedHeight = 30
        };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Terrain Generation",headerStyle);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        creator.TFrequency = EditorGUILayout.FloatField(new GUIContent("Noise Frequency",
            "This value is used to determine the frequency of the noise, higher values mean more changes in the terrain."), creator.TFrequency);

        creator.NScale = EditorGUILayout.FloatField(new GUIContent("Noise Scale",
            "This value is used to determine the size and variations in the height of the terrain."), creator.NScale);

        creator.TDetails = EditorGUILayout.IntSlider(new GUIContent("Collider Density",
            "This value is used to determine how smooth your terrain is, higher values mean more points per segment."),creator.TDetails, 1, 100);

        creator.ConnectSubPaths = EditorGUILayout.Toggle(new GUIContent("Connect Paths",
            "This value determines if the colliders of the subpaths get connected to where they branch from."), creator.ConnectSubPaths);

        if (GUILayout.Button(new GUIContent("Reset points",
            "This will reset all the points of your base terrain lines.")))
        {
            Undo.RecordObject(creator, "Points Reset");
            creator.GeneratePath();
            creator.ResetSubPaths();
            path = creator.path;
            creator.UpdateCollider();
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Terrain Limits",headerStyle);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        creator.StartTerrain = (PathGenerator.SEType)EditorGUILayout.EnumPopup(new GUIContent("Start Type", "Determines how the terrain starts."),creator.StartTerrain);

        creator.EndTerrain = (PathGenerator.SEType)EditorGUILayout.EnumPopup(new GUIContent("End Type", "Determines how the terrain ends."),creator.EndTerrain);

        creator.VOffset = EditorGUILayout.FloatField(new GUIContent("Start/End Size",
            "This value is used for the size of the collider at the start and end of your terrain. Size is measured in Unity units."), creator.VOffset);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Seed Generation",headerStyle);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");    
        creator.Seed = EditorGUILayout.IntField(new GUIContent("Seed",
            "This value is used to generate a unique noise with your given values."),creator.Seed);
        if (GUILayout.Button(new GUIContent("New Seed",
            "This will generate a completely new seed number.")))
        {
            Undo.RecordObject(creator, "New Seed");
            creator.GenerateNewSeed();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Options",headerStyle);
        EditorGUILayout.Space();


        EditorGUILayout.BeginVertical("box");
        creator.DoHandleSize = EditorGUILayout.FloatField(new GUIContent("Point Size",
            "This value is used for the size of the collider at the start and end of your terrain. Size is measured in Unity units."), creator.DoHandleSize);

        if (GUILayout.Button("Open Terrain Exporter"))
        {
            ExporterEditor.terrain.currentPath = creator;
            ExporterEditor.ShowExporter();
        }
        EditorGUILayout.EndVertical();

        creator.UpdateCollider();
    }

    void OnSceneGUI()
    {
        Input();
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        //Adding point to the path or sub path
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            //Check main path for closest point first
            int selectedPointIndex = -1;

            float smallestDist = Mathf.Infinity;
            for (int i = 0; i < path.NumPoints; i++)
            {
                float dist = Vector2.Distance(mousePos, path[i]);
                if (dist < smallestDist)
                {
                    selectedPointIndex = i;
                    smallestDist = dist;                    
                }
            }

            //Check sub paths for closest point
            int selectedSubPath = -1;
            for (int i = 0; i < creatorSubPaths.Count; i++)
            {
                for (int j = 1; j < creatorSubPaths[i].NumPoints; j++)
                {
                    float dist = Vector2.Distance(mousePos, creatorSubPaths[i][j]);
                    if (dist < smallestDist)
                    {
                        selectedPointIndex = j;
                        selectedSubPath = i;
                        smallestDist = dist;
                    }
                }
            }

            //If there were no subpaths, head straight to the main path
            if (selectedSubPath == -1)
            {
                if(selectedPointIndex == 0)
                {
                    Undo.RecordObject(creator, "Add point at start");
                    path.AddPointStart(mousePos);
                    creator.UpdateCollider();
                }
                else if(selectedPointIndex == path.NumPoints - 1)
                {
                    Undo.RecordObject(creator, "Add point at end");
                    path.AddPoint(mousePos);
                    creator.UpdateCollider();
                }
                else
                {
                    Undo.RecordObject(creator, "Main path, sub path");
                    creator.GenerateSubPath(path[selectedPointIndex], mousePos);
                    //creator.subColliders.Add(Undo.AddComponent<EdgeCollider2D>(creator.gameObject));
                    creator.UpdateCollider();
                }
            }

            //Since there was a subpath choosen closer than the last point by the mainpath, head straight to the selected path
            else
            {
                if (selectedPointIndex == creatorSubPaths[selectedSubPath].NumPoints - 1)
                {
                    Undo.RecordObject(creator, "Add point at end");
                    creatorSubPaths[selectedSubPath].AddPoint(mousePos);
                    creator.UpdateCollider();
                }
                else
                {
                    Undo.RecordObject(creator, "Sub path, sub path");
                    creator.GenerateSubPath(creatorSubPaths[selectedSubPath][selectedPointIndex], mousePos);
                    //creator.subColliders.Add(Undo.AddComponent<EdgeCollider2D>(creator.gameObject));
                    creator.UpdateCollider();
                }
            }        
        }

        //When deleting point in path or sub path
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.shift)
        {
            float minDstToPoint = creator.DoHandleSize * 1.2f;
            int closestPointIndex = -1;

            for (int i = 0; i < path.NumPoints; i++)
            {
                float dist = Vector2.Distance(mousePos, path[i]);
                if (dist < minDstToPoint)
                {
                    minDstToPoint = dist;
                    closestPointIndex = i;
                }
            }

            int selectedSubPath = -1;
            for (int i = 0; i < creatorSubPaths.Count; i++)
            {
                for (int j = 1; j < creatorSubPaths[i].NumPoints; j++)
                {
                    float dist = Vector2.Distance(mousePos, creatorSubPaths[i][j]);
                    if (dist < minDstToPoint)
                    {
                        closestPointIndex = j;
                        selectedSubPath = i;
                        minDstToPoint = dist;
                    }
                }
            }

            //Add a function inside path Generator that makes subpaths attach to the point previous to the one deleted.
            if (closestPointIndex != -1)
            {
                if (selectedSubPath != -1)
                {
                    Undo.RecordObject(creator, "Remove point");
                    creatorSubPaths[selectedSubPath].RemovePoint(closestPointIndex);
                    creator.UpdateCollider();

                    //When deleting the last visible point from the subpath, it will delete the left over point at the branching point
                    if (creatorSubPaths[selectedSubPath].NumPoints == 1)
                    {  
                        creator.RemoveSubPoint(creator.GetSubPointAt(creatorSubPaths[selectedSubPath][0]));
                        creatorSubPaths.RemoveAt(selectedSubPath);
                        Undo.DestroyObjectImmediate(creator.subColliders[selectedSubPath]);
                        creator.subColliders.RemoveAt(selectedSubPath);
                    }
                }
                else
                {
                    //Prevents from deleting the last point in the main path
                    if (path.NumPoints != 1)
                    {
                        Undo.RecordObject(creator, "Remove point");
                        path.RemovePoint(closestPointIndex);
                        creator.UpdateCollider();
                    }
                }
            }
        }
    }

    void Draw()
    {
        //Draw the main path lines
        Vector2[] points = path.GetPoints;
        for (int i = 0; i < path.NumSegments; i++)
        {
            Handles.DrawLine(points[i], points[i + 1]);
        }

        //Draw the sub path lines
        for (int i = 0; i < creatorSubPaths.Count; i++)
        {
            Vector2[] subPoints = creatorSubPaths[i].GetPoints;
            for (int j = 0; j < creatorSubPaths[i].NumSegments; j++)
            {
                Handles.DrawLine(subPoints[j], subPoints[j + 1]);
            }
        }

        //Draw handles of main path, check for movement
        Handles.color = Color.cyan;
        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector2 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, creator.DoHandleSize, Vector2.one, Handles.CylinderHandleCap);
            //Vector2 newPos = Handles.FreeMoveHandle(path[i] + (Vector2)creator.gameObject.transform.position, Quaternion.identity, creator.DoHandleSize, Vector2.one, Handles.CylinderHandleCap) - creator.gameObject.transform.position;
            //This updates handles position with the game object, but not sure how to apply it to the path points. Unused for now.

            //Check if the handle has been moved
            if (path[i] != newPos)
            {
                Vector2 originalPos = path[i];

                //Check to see if the handle has any subPaths starting from them
                for(int j = 0; j < creator.NumSubPoints; j++)
                {
                    if(originalPos == creator.GetSubPoint(j))
                    {
                        Undo.RecordObject(creator, "Move point");
                        path.MovePoint(i, newPos);
                        creator.MoveSubPoint(j, newPos);
                    }
                }
                if (path[i] != newPos)
                {
                    Undo.RecordObject(creator, "Move point");
                    path.MovePoint(i, newPos);
                }
            }
        }

        //Draw handles of sub paths, check for movement
        Handles.color = Color.blue;
        for (int i = 0; i < creatorSubPaths.Count; i++)
        {
            for (int j = 1; j < creatorSubPaths[i].NumPoints; j++)
            {
                Vector2 newPos = Handles.FreeMoveHandle(creatorSubPaths[i][j], Quaternion.identity, creator.DoHandleSize, Vector2.one, Handles.CylinderHandleCap);

                //Check if Handle has been moved
                if (creatorSubPaths[i][j] != newPos)
                {
                    Vector2 originalPos = creatorSubPaths[i][j];
                    
                    //Check to see if the handle has any subPaths starting from them
                    for (int k = 0; k < creator.NumSubPoints; k++)
                    {
                        if (originalPos == creator.GetSubPoint(k))
                        {
                            Undo.RecordObject(creator, "Move point");
                            creatorSubPaths[i].MovePoint(j, newPos);
                            creator.MoveSubPoint(k, newPos);
                        }
                    }
                    if (creatorSubPaths[i][j] != newPos)
                    {
                        Undo.RecordObject(creator, "Move point");
                        creatorSubPaths[i].MovePoint(j, newPos);
                    }
                }
            }
        }

        //Event guiEvent = Event.current;
        //Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        //Handles.Label(mousePos, mousePos.ToString());
    }

    void OnEnable()
    {
        creator = (PathGenerator)target;
        if(creator.path == null)
        {
            creator.GeneratePath();
        }
        path = creator.path;
        creatorSubPaths = creator.subPaths;
    }
}
