using System.Collections;
using UnityEditor;
using UnityEngine;

public class ExporterEditor : EditorWindow
{

    public static TerrainExporter terrain = new TerrainExporter();

    public static void ShowExporter()
    {
        ExporterEditor window = GetWindow<ExporterEditor>("Terrain Exporter");
    }

    void OnGUI()
    {
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Background Color");
        terrain.BGColor = EditorGUILayout.ColorField(terrain.BGColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Collider Line Color");
        terrain.LineColor = EditorGUILayout.ColorField(terrain.LineColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("File name");
        terrain.Name = EditorGUILayout.TextField(terrain.Name);
        EditorGUILayout.EndHorizontal();
    
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Pixels per Unit");
        terrain.PixelsPerUnit = EditorGUILayout.IntField(terrain.PixelsPerUnit);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Export as png"))
        {
            GUIContent gUI = new GUIContent
            {
                text = "Exported!"
            };
            GetWindow<ExporterEditor>().ShowNotification(gUI);
            terrain.ExportToPng();
        }
    }
}

