using System.Collections;
using UnityEditor;
using UnityEngine;

public class ExporterEditor : EditorWindow
{

    TerrainExporter terrain;

    public static void ShowExporter()
    {
        ExporterEditor window = GetWindow<ExporterEditor>("Terrain Exporter");
        window.minSize = new Vector2(600, 300);
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

        if (GUILayout.Button("Export as png"))
        {
            terrain.ExportToPng();
        }
    }
}
