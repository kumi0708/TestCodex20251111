using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Run100SceneSetup
{
    // Executed via: -executeMethod Run100SceneSetup.Setup
    public static void Setup()
    {
        var scenePath = "Assets/Scenes/run100.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        // Clear existing non-essential objects (optional) – keep Camera/Light if present
        // We will manage/create what we need.

                // Cleanup previously created objects
        foreach (var go in scene.GetRootGameObjects())
        {
            if (go.name == "Ground" || go.name == "RaceManager" || go.name == "Canvas" || go.name == "StatusText" || go.GetComponent<Runner>() != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        // Ground (centered at z=50, covers 0..100 with margin)
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, 0f, 50f);
        ground.transform.localScale = new Vector3(2f, 1f, 15f); // ~20 x 150 units

        // Runners
        CreateRunner(PrimitiveType.Sphere, new Vector3(-2f, 0.5f, 0f), "Sphere");
        CreateRunner(PrimitiveType.Cube, new Vector3(0f, 0.5f, 0f), "Cube");
        CreateRunner(PrimitiveType.Capsule, new Vector3(2f, 1.0f, 0f), "Capsule");
        // Manager
        var managerGO = new GameObject("RaceManager");
        var manager = managerGO.AddComponent<RaceManager>();

        // UI Canvas + Text
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var textGO = new GameObject("StatusText", typeof(Text));
        textGO.transform.SetParent(canvasGO.transform, false);
        var text = textGO.GetComponent<Text>();
        text.alignment = TextAnchor.UpperLeft;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.color = Color.white;
        text.text = "準備中";
        var rt = text.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        rt.sizeDelta = new Vector2(800, 200);

        managerGO.GetComponent<RaceManager>().GetType()
            .GetField("statusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(manager, text);

        // Camera
        Camera cam = Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera", typeof(Camera));
            cam = camGO.GetComponent<Camera>();
            cam.tag = "MainCamera";
        }
        cam.transform.position = new Vector3(0, 12, -20);
        cam.transform.LookAt(new Vector3(0, 0, 0));
        var follow = cam.gameObject.GetComponent<FollowCameraController>();
        if (follow == null) follow = cam.gameObject.AddComponent<FollowCameraController>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.Refresh();
    }

    static void CreateRunner(PrimitiveType type, Vector3 position, string name)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.position = position;
        var r = go.AddComponent<Runner>();
        r.runnerName = name;
        // Simple coloring for identification
        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            var mat = new Material(Shader.Find("Standard"));
            if (name == "Sphere") mat.color = Color.red;
            else if (name == "Cube") mat.color = Color.green;
            else mat.color = Color.blue;
            renderer.sharedMaterial = mat;
        }
    }
}


