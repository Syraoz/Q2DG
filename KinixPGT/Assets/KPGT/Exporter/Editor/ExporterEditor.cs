using System.Collections;
using UnityEditor;
using UnityEngine;

public class ExporterEditor : EditorWindow
{

    public static TerrainExporter terrain = new TerrainExporter();

    public static void ShowExporter()
    {
        ExporterEditor window = GetWindow<ExporterEditor>("Terrain Exporter");
        window.minSize = new Vector2(500, 250);
    }

    void OnGUI()
    {
        GUILayout.Label("Exporter");
        GUILayout.Label(terrain.Name);

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Background Color");
        terrain.BGColor = EditorGUILayout.ColorField(terrain.BGColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Collider Line Color");
        terrain.LineColor = EditorGUILayout.ColorField(terrain.LineColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("File Name");
        terrain.Name = EditorGUILayout.TextField(terrain.Name);
        EditorGUILayout.EndHorizontal();
    
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Pixels per Unit");
        terrain.PixelsPerUnit = EditorGUILayout.IntField(terrain.PixelsPerUnit);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Image Margin");
        terrain.Margin = EditorGUILayout.IntField(terrain.Margin);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Export as .png"))
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

