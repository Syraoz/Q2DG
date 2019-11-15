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
        if (terrain.currentPath != null)
        {
            GUILayout.Label(terrain.currentPath.name + " is currently selected");
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("File Name:", "Name the file of the exported image."));
            terrain.Name = EditorGUILayout.TextField(terrain.Name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Export as...", "Format of the image to be exported."));
            terrain.ImageFormat = (TerrainExporter.ExportFormat)EditorGUILayout.EnumPopup(terrain.ImageFormat);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Background Color");
            terrain.BGColor = EditorGUILayout.ColorField(terrain.BGColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Collider Line Color");
            terrain.LineColor = EditorGUILayout.ColorField(terrain.LineColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Pixels per Unit","Amount of pixels per unit of Unity. The standard is 100 pixels per unit."));
            terrain.PixelsPerUnit = EditorGUILayout.IntField(terrain.PixelsPerUnit);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Image Margin", "Margin from the border of the image to the first pixel of the collider in the image."));
            terrain.Margin = EditorGUILayout.IntField(terrain.Margin);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Export terrain"))
            {
                if (terrain.ExportTerrainAs())
                {
                    GUIContent gUI = new GUIContent
                    {
                        text = "Successfully exported terrain"
                    };
                    GetWindow<ExporterEditor>().ShowNotification(gUI);
                    AssetDatabase.Refresh();
                    
                }
                else
                {
                    GUIContent gUI = new GUIContent
                    {
                        text = "Unable to Export terrain"
                    };
                    GetWindow<ExporterEditor>().ShowNotification(gUI);
                }
            }
        }
        else
        {
            GUILayout.Label("Please select a terrain first");
        }
    }
}


