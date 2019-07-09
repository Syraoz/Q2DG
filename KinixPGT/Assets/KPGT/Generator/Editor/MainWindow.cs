using UnityEditor;
using UnityEngine;

public class MainWindow : EditorWindow
{

    [MenuItem("Tools/KinixPGT")]
    public static void ShowWindow()
    {
        MainWindow window = GetWindow<MainWindow>("KinixPGT");
        window.minSize = new Vector2(600, 300);
        
    }

    void OnGUI()
    {
        GUILayout.Label("Welcome to the prototype version of PGOT!");
        if (GUILayout.Button("Generate")){

            GameObject newTerrain = new GameObject();
            newTerrain.name = "New 2DTerrain";
            newTerrain.AddComponent<PathGenerator>();

            if (newTerrain.GetComponent<PathGenerator>() != null)
            {
                GUIContent gUI = new GUIContent
                {
                    text = "Base terrain generated!"
                };
                GetWindow<MainWindow>("KinixPGT").ShowNotification(gUI);
            }
        }
    }


}
