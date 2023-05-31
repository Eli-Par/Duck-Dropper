using UnityEngine;
using UnityEditor;

public class Renaming : EditorWindow
{

    [MenuItem("Window/Section Editor")]
    public static void ShowWindow()
    {
        GetWindow<Renaming>("Section Editor");
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Hide Culled Selected Sections"))
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
