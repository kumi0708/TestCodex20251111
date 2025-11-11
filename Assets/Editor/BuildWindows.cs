using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildWindows
{
    // Run via: -executeMethod BuildWindows.Run
    public static void Run()
    {
        string scenePath = "Assets/Scenes/run100.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string outDir = Path.Combine(projectRoot, $"Run_{timestamp}");
        Directory.CreateDirectory(outDir);

        string exeName = "TestCodex.exe";
        string exePath = Path.Combine(outDir, exeName);

        var options = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed: {report.summary.result} - {report.summary.totalErrors} errors");
        }
        Debug.Log($"Build succeeded: {exePath}");
    }
}
