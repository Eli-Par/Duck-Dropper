using UnityEngine;
using UnityEditor;

public class Renaming : EditorWindow
{
    string newName = "Object";

    [MenuItem("Window/Renaming")]
    public static void ShowWindow()
    {
        GetWindow<Renaming>("Renaming");
    }

    private void OnGUI()
    {
        //GUILayout.Label("Name", EditorStyles.boldLabel);
        newName = EditorGUILayout.TextField("Name", newName);

        if(GUILayout.Button("Something Selected"))
        {
            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Duck_Section section = Selection.gameObjects[i].GetComponent<Duck_Section>();
                Renderer renderer = Selection.gameObjects[i].GetComponent<Renderer>();

                if(section != null && !section.cullingEnabled)
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
