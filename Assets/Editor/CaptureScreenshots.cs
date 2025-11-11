using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CaptureScreenshots
{
    // Run via: -executeMethod CaptureScreenshots.Capture
    public static void Capture()
    {
        string scenePath = "Assets/Scenes/run100.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var docs = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "docs");
        Directory.CreateDirectory(docs);

        Camera cam = Object.FindObjectOfType<Camera>();
        if (cam == null) cam = new GameObject("TempCamera", typeof(Camera)).GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.2f, 0.3f, 0.45f);
        cam.fieldOfView = 60f;

        // Ensure at least one directional light exists
        var light = Object.FindObjectOfType<Light>();
        if (light == null)
        {
            var lg = new GameObject("Directional Light", typeof(Light));
            light = lg.GetComponent<Light>();
            light.type = LightType.Directional;
            lg.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 1.1f;
        }

        var runners = Object.FindObjectsOfType<Runner>();
        var original = new System.Collections.Generic.Dictionary<Runner, Vector3>();
        foreach (var r in runners) original[r] = r.transform.position;

        int width = 1920, height = 1080;

        // View 1: 高めの俯瞰（全体）
        cam.transform.position = new Vector3(0f, 18f, -35f);
        cam.transform.LookAt(new Vector3(0f, 0f, 60f));
        SaveFromCamera(cam, width, height, Path.Combine(docs, "screenshot_overview.png"));

        // View 2: 斜め後方（途中まで進めた状態を演出）
        float z0 = 40f;
        for (int i = 0; i < runners.Length; i++)
        {
            var rp = runners[i].transform.position;
            runners[i].transform.position = new Vector3(rp.x, rp.y, z0 - i * 5f);
        }
        cam.transform.position = new Vector3(-12f, 12f, 10f);
        cam.transform.LookAt(new Vector3(0f, 1f, 55f));
        SaveFromCamera(cam, width, height, Path.Combine(docs, "screenshot_angle.png"));

        // View 3: スタート付近（開始時の配置）
        for (int i = 0; i < runners.Length; i++)
        {
            var rp = runners[i].transform.position;
            runners[i].transform.position = new Vector3(rp.x, rp.y, 0f);
        }
        cam.transform.position = new Vector3(0f, 10f, -20f);
        cam.transform.LookAt(new Vector3(0f, 0.5f, 0f));
        SaveFromCamera(cam, width, height, Path.Combine(docs, "screenshot_start.png"));

        // View 4: ゴール付近（ゴール手前/直後の様子）
        float g0 = 100f;
        if (runners.Length >= 3)
        {
            runners[0].transform.position = new Vector3(runners[0].transform.position.x, runners[0].transform.position.y, g0);
            runners[1].transform.position = new Vector3(runners[1].transform.position.x, runners[1].transform.position.y, g0 - 3f);
            runners[2].transform.position = new Vector3(runners[2].transform.position.x, runners[2].transform.position.y, g0 - 6f);
        }
        cam.transform.position = new Vector3(0f, 12f, 130f);
        cam.transform.LookAt(new Vector3(0f, 1f, 100f));
        SaveFromCamera(cam, width, height, Path.Combine(docs, "screenshot_goal.png"));

        // 元に戻す（シーンは保存しない）
        foreach (var kv in original) kv.Key.transform.position = kv.Value;
        Debug.Log("Screenshots saved to docs folder.");
    }

    static void SaveFromCamera(Camera cam, int width, int height, string path)
    {
        var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        var prevTarget = cam.targetTexture;
        var prevActive = RenderTexture.active;
        try
        {
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            var png = tex.EncodeToPNG();
            File.WriteAllBytes(path, png);
        }
        finally
        {
            cam.targetTexture = prevTarget;
            RenderTexture.active = prevActive;
        }
    }
}
