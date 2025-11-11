using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class ProjectSetup
{
    // Executed via: -executeMethod ProjectSetup.CreateRun100Scene
    public static void CreateRun100Scene()
    {
        var scenesDir = "Assets/Scenes";
        if (!Directory.Exists(scenesDir)) Directory.CreateDirectory(scenesDir);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var path = Path.Combine(scenesDir, "run100.unity");
        if (!EditorSceneManager.SaveScene(scene, path))
        {
            throw new System.Exception("Failed to save scene to " + path);
        }
        AssetDatabase.Refresh();
    }
}
