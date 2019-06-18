using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EdgeBuilder))]
public class MainWindow : EditorWindow
{
    [SerializeField]
    Vector2 start;
    Vector2 end;

    [MenuItem("Window/KinixPGT")]
    public static void ShowWindow()
    {
        
        GetWindow<MainWindow>("KinixPGT");
    }

    void OnGUI()
    {
        GUILayout.Label("Welcome to the prototype version of PGOT!");

        EdgeBuilder builder = new EdgeBuilder();

        start = EditorGUILayout.Vector2Field("Start:", start);
        end = EditorGUILayout.Vector2Field("End:", end);

        if (GUILayout.Button("Generate")){
            builder.GenerateBase(start,end);
        }
    }


}
