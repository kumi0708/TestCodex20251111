using UnityEngine;

public class Runner : MonoBehaviour
{
    public string runnerName = "Runner";
    public float minSpeed = 6f;
    public float maxSpeed = 10f;
    public float variability = 0.5f; // small speed wobble

    [HideInInspector] public float finishTime = -1f;

    RaceManager manager;
    float baseSpeed;
    float distanceTravelled;
    Vector3 startPosition;

    public float Progress => distanceTravelled;

    public void Initialize(RaceManager mgr)
    {
        manager = mgr;
        startPosition = transform.position;
        distanceTravelled = 0f;
        baseSpeed = Random.Range(minSpeed, maxSpeed);
        finishTime = -1f;
    }

    void Update()
    {
        if (manager == null || !manager.RaceStarted || finishTime >= 0f) return;

        float wobble = Mathf.Sin(Time.time * 2.3f + transform.GetInstanceID()) * variability;
        float speed = Mathf.Max(0f, baseSpeed + wobble);
        float step = speed * Time.deltaTime;

        distanceTravelled += step;
        Vector3 pos = transform.position;
        pos.z = startPosition.z + distanceTravelled;
        transform.position = pos;

        if (distanceTravelled >= manager.RaceDistance)
        {
            distanceTravelled = manager.RaceDistance;
            pos.z = startPosition.z + distanceTravelled;
            transform.position = pos;
            finishTime = Time.time - manager.RaceStartTime;
            manager.NotifyFinished(this);
        }
    }
}
