using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EdgeBuilder))]
public class MainWindow : EditorWindow
{

    [MenuItem("Window/KinixPGT")]
    public static void ShowWindow()
    {
        GetWindow<MainWindow>("KinixPGT");
    }

    void OnGUI()
    {
        GUILayout.Label("Welcome to the prototype version of PGNT!");

        EdgeBuilder builder = new EdgeBuilder();

        if (GUILayout.Button("Generate")){
            builder.GenerateBase();
        }
    }


}
