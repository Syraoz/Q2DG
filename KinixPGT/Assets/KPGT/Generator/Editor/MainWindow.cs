using UnityEditor;
using UnityEngine;

public class MainWindow : EditorWindow
{

    private string objectName;
    private Vector2 pos;

    [MenuItem("Tools/KPGT")]
    public static void ShowWindow()
    {
        MainWindow window = GetWindow<MainWindow>("KPGT");
        window.minSize = new Vector2(600, 350);
        
    }

    void OnGUI()
    {
        GUILayout.Label("Terrain Setup");

        GUILayout.Label("Terrain Name:");
        objectName = GUILayout.TextField(objectName);

        if (GUILayout.Button("Create Base")){

            GameObject newTerrain = new GameObject();
            newTerrain.name = objectName;
            newTerrain.AddComponent<PathGenerator>();

            if (newTerrain.GetComponent<PathGenerator>() != null)
            {
                GUIContent gUI = new GUIContent
                {       
                    text = "Terrain Gameobject Created"
                };
                GetWindow<MainWindow>("KinixPGT").ShowNotification(gUI);
                Selection.activeGameObject = newTerrain;
            }
        }

        GUILayout.Space(50);

        GUILayout.Label("While holding shift:");
        GUILayout.Label("Left-click to create a new point from the last one.");
        GUILayout.Label("Right-click on a point to erase it.");

        GUILayout.Space(130);

        GUILayout.Label("KPGT Beta v1.4");
    }
}
