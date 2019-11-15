using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGenerator))]
public class PathEditor : Editor
{
    PathGenerator creator;
    Path path;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /*if(GUILayout.Button("Generate Terrain"))
        {
            Undo.RecordObject(creator, "Terrain Altered");
            creator.RandomizeTerrain();
        }*/

        EditorGUILayout.BeginHorizontal();
        creator.TAmplitude = EditorGUILayout.FloatField(new GUIContent("Detail of Frequency",
            "This value is used to determine the frequency of the noise, higher values mean more changes in the terrain."), creator.TAmplitude);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Collider Density", "This value is used to determine how smooth your terrain is, higher values mean more points per segment."));
        creator.TFrequency = EditorGUILayout.IntSlider(creator.TFrequency,0,100);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        creator.VOffset = EditorGUILayout.FloatField(new GUIContent("Start/End Size", 
            "This value is used for the size of the collider at the start and end of your terrain. Size is measured in Unity units."), creator.VOffset);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Seed",
            "This value is used to generate a unique noise with your given values."));
        if (GUILayout.Button(new GUIContent("New Seed",
             "This will generate a completely new seed number.")))
        {
            Undo.RecordObject(creator, "New Seed");
            creator.GenerateNewSeed();
        }
        creator.Seed = EditorGUILayout.IntField(creator.Seed);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Start Type","Determines how the terrain starts."));
        creator.StartTerrain = (PathGenerator.SEType)EditorGUILayout.EnumPopup(creator.StartTerrain);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("End Type","Determines how the terrain ends."));
        creator.EndTerrain = (PathGenerator.SEType)EditorGUILayout.EnumPopup(creator.EndTerrain);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();


        if (GUILayout.Button(new GUIContent(creator.GetComponent<EdgeCollider2D>() != null ? "Disable Collider" : "Enable Collider",
            "Disables or enables the collider of the gameobject.")))
        {
            creator.ToggleCollider();
        }
            
        if (GUILayout.Button(new GUIContent("Reset points",
            "This will reset all the points of your base terrain lines.")))
        {
            Undo.RecordObject(creator, "Points reset");
            creator.GeneratePath();
            path = creator.path;
            creator.UpdateCollider();
            SceneView.RepaintAll();
        }

        creator.UpdateCollider();

        if (GUILayout.Button("Open Terrain Exporter"))
        {
            ExporterEditor.terrain.currentPath = creator;
            ExporterEditor.ShowExporter();
        }


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

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(creator, "Add point");
            path.AddPoint(mousePos);
            creator.UpdateCollider();

        }
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.shift)
        {
            float minDstToPoint = 0.15f;
            int closestPointIndex = -1;

            for(int i = 0; i < path.NumPoints; i++)
            {
                float dist = Vector2.Distance(mousePos, path[i]);
                if (dist < minDstToPoint)
                {
                    minDstToPoint = dist;
                    closestPointIndex = i;
                }
            }
            if(closestPointIndex != -1)
            {
                Undo.RecordObject(creator, "Remove point");
                path.RemovePoint(closestPointIndex);
                creator.UpdateCollider();
            }
        }
    }

    void Draw()
    {
        Vector2[] points = path.GetPoints;
        for(int i = 0; i < path.NumSegments; i++)
        {
            Handles.DrawLine(points[i], points[i + 1]);
        }

        Handles.color = Color.cyan;
        for(int i = 0; i < path.NumPoints; i++)
        {
            Vector2 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 0.1f, Vector2.one, Handles.CylinderHandleCap);
            if(path[i] != newPos)
            {
                Undo.RecordObject(creator, "Move point");
                path.MovePoint(i, newPos);
            }
        }
    }

    void OnEnable()
    {
        creator = (PathGenerator)target;
        if(creator.path == null)
        {
            creator.GeneratePath();
        }
        path = creator.path;
    }
}
