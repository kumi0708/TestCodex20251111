using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    [Header("Race Settings")]
    [SerializeField] float raceDistance = 100f;

    [Header("References")]
    [SerializeField] Text statusText;

    public float RaceDistance => raceDistance;
    public bool RaceStarted { get; private set; }
    public float RaceStartTime { get; private set; }

    List<Runner> runners = new List<Runner>();
    int finishedCount = 0;

    public IReadOnlyList<Runner> Runners => runners;

    void Start()
    {
        // Find all runners by component
        runners = FindObjectsOfType<Runner>().OrderBy(r => r.transform.position.x).ToList();
        foreach (var r in runners) r.Initialize(this);

        StartRace();
    }

    public void StartRace()
    {
        RaceStartTime = Time.time;
        RaceStarted = true;
        finishedCount = 0;
        UpdateStatus("スタート！ 100メートル競争");
    }

    public void NotifyFinished(Runner r)
    {
        finishedCount++;
        if (finishedCount == runners.Count)
        {
            RaceStarted = false;
            ShowResults();
        }
    }

    void Update()
    {
        // Escape key to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return;
        }

        if (!RaceStarted)
        {
            return;
        }

        var last = GetLastPlace();
        var first = GetFirstPlace();
        if (last != null && first != null)
        {
            UpdateStatus($"先頭: {first.runnerName}  最下位: {last.runnerName}");
        }
    }

    Runner GetLastPlace()
    {
        if (runners == null || runners.Count == 0) return null;
        return runners.OrderBy(r => r.Progress).First();
    }

    Runner GetFirstPlace()
    {
        if (runners == null || runners.Count == 0) return null;
        return runners.OrderByDescending(r => r.Progress).First();
    }

    void ShowResults()
    {
        var ordered = runners.OrderBy(r => r.finishTime).ToList();
        string msg = "結果\n";
        for (int i = 0; i < ordered.Count; i++)
        {
            var r = ordered[i];
            msg += $"{i + 1}位: {r.runnerName}  タイム: {r.finishTime:0.00}s\n";
        }
        UpdateStatus(msg);
        Debug.Log(msg);
    }

    void UpdateStatus(string msg)
    {
        if (statusText != null)
        {
            statusText.text = msg;
        }
    }
}
