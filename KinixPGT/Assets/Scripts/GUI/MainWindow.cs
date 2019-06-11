using UnityEditor;
using UnityEngine;

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

        if (GUILayout.Button("Generate")){

        }
    }


}
